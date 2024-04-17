
using System.Text;
using System.Text.Json;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers
{
  public static class Handler
  {
    public static IResult getPersons(ApplicationContext context)
    {
      var personsWithInterests = context.Person
                  .Include(p => p.PersonInterestLinks)
                      .ThenInclude(pi => pi.Interest)
                  .Select(p => new
                  {
                    PersonID = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PhoneNumber = p.PhoneNumber,
                    Interests = p.PersonInterestLinks.Select(pi => new
                    {
                      InterestID = pi.Interest.Id,
                      Title = pi.Interest.Title,
                      Description = pi.Interest.Description
                    }).ToList()
                  })
                  .ToList();

      return Results.Json(personsWithInterests);
    }

    public static IResult getInterestsForPerson(ApplicationContext context, int personId)
    {
      var InterestsOfAPerson = context.Person
                  .Include(p => p.PersonInterestLinks)
                      .ThenInclude(pi => pi.Interest)
                  .Where(p => p.Id == personId)
                  .Select(p => new
                  {
                    PersonID = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PhoneNumber = p.PhoneNumber,
                    Interests = p.PersonInterestLinks.Select(pi => new
                    {
                      InterestID = pi.Interest.Id,
                      Title = pi.Interest.Title,
                      Description = pi.Interest.Description
                    }).ToList()
                  })
                  .ToList();

      return Results.Json(InterestsOfAPerson);
    }

    public static IResult getLinksForPerson(ApplicationContext context, int personId)
    {
      var LinksOfAPerson = context.Person
                  .Include(p => p.Links)
                  .Where(p => p.Id == personId)
                  .Select(p => new
                  {
                    PersonID = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PhoneNumber = p.PhoneNumber,
                    Links = p.Links.Select(l => new
                    {
                      LinkID = l.Id,
                      Url = l.Url
                    }).ToList()
                  })
                  .ToList();

      return Results.Json(LinksOfAPerson);
    }

    public static async Task<IResult> addInterestToPerson(ApplicationContext context, int personId, HttpContext httpContext)
    {
      try
      {
        using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
        {
          var requestBody = await reader.ReadToEndAsync();

          var json = JsonSerializer.Deserialize<JsonDocument>(requestBody);

          var interestId = json.RootElement.GetProperty("interestId").GetString();

          if (string.IsNullOrEmpty(interestId))
          {
            return Results.BadRequest("InterestId is required");
          }

          var person = context.Person
              .Include(p => p.PersonInterestLinks)
              .Where(p => p.Id == personId)
              .FirstOrDefault();

          var interest = context.Interest
              .Where(i => i.Id == Convert.ToInt32(interestId))
              .FirstOrDefault();

          if (person == null || interest == null)
          {
            return Results.BadRequest("Person or Interest not found");
          }

          var personInterestLink = new PersonInterestLink
          {
            Person = person,
            Interest = interest
          };

          context.PersonInterestLink.Add(personInterestLink);
          context.SaveChanges();

          return Results.Ok();
        }
      }
      catch (Exception ex)
      {
        return Results.BadRequest("Invalid request.");
      }
    }

    public static async Task<IResult> addLinksToPersonInterest(HttpContext httpContext, ApplicationContext context, int personId, int interestId)
    {
      try
      {
        using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
        {
          var requestBody = await reader.ReadToEndAsync();

          var json = JsonSerializer.Deserialize<JsonDocument>(requestBody);

          var linksArray = json.RootElement.GetProperty("links").EnumerateArray();

          var person = context.Person
              .Include(p => p.PersonInterestLinks)
              .FirstOrDefault(p => p.Id == personId);

          var interest = context.Interest
              .Include(i => i.PersonInterestLinks)
              .FirstOrDefault(i => i.Id == interestId);

          if (person == null || interest == null)
          {
            return Results.BadRequest("Person or Interest not found");
          }

          foreach (var linkElement in linksArray)
          {
            var linkUrl = linkElement.GetProperty("url").GetString();

            if (string.IsNullOrEmpty(linkUrl))
            {
              return Results.BadRequest("Link URL is required");
            }

            var link = new Link
            {
              Url = linkUrl,
              Person = person,
              Interest = interest
            };

            context.Link.Add(link);
          }

          context.SaveChanges();

          return Results.Ok();
        }
      }
      catch (Exception ex)
      {
        return Results.BadRequest("Invalid request.");
      }
    }

  }
}