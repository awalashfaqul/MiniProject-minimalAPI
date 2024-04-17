
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
            // Retrieve all persons from the database, including their associated interests
            var personsWithInterests = context.Person
                        .Include(p => p.PersonInterestLinks) // Include the PersonInterestLinks navigation property to load related interests
                            .ThenInclude(pi => pi.Interest) // Then include the Interest navigation property within PersonInterestLinks
                        .Select(p => new // Project each person into an anonymous type with specified properties
                        {
                            PersonID = p.Id, // Store the ID of the person
                            FirstName = p.FirstName, // Store the first name of the person
                            LastName = p.LastName, // Store the last name of the person
                            PhoneNumber = p.PhoneNumber, // Store the phone number of the person
                            Interests = p.PersonInterestLinks.Select(pi => new // Project each associated interest into an anonymous type
                            {
                                InterestID = pi.Interest.Id, // Store the ID of the interest
                                Title = pi.Interest.Title, // Store the title of the interest
                                Description = pi.Interest.Description // Store the description of the interest
                            }).ToList() // Convert the interests into a list
                        })
                        .ToList(); // Execute the query and retrieve the result as a list

            // Return a JSON result containing the retrieved persons with their associated interests
            return Results.Json(personsWithInterests);
        }



        public static IResult getInterestsForPerson(ApplicationContext context, int personId)
        {
            // Retrieve interests associated with a specific person from the database
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

                        // Project each associated interest into an anonymous type
                        Interests = p.PersonInterestLinks.Select(pi => new
                        {
                          InterestID = pi.Interest.Id,
                          Title = pi.Interest.Title,
                          Description = pi.Interest.Description
                        }).ToList()
                      })
                      .ToList();

            // Return a JSON result containing the retrieved interests associated with the person
            return Results.Json(InterestsOfAPerson);
        }

        public static IResult getLinksForPerson(ApplicationContext context, int personId)
        {
            // Retrieve links associated with a specific person from the database
            var LinksOfAPerson = context.Person
                      .Include(p => p.Links)
                      .Where(p => p.Id == personId)
                      .Select(p => new
                      {
                        PersonID = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        PhoneNumber = p.PhoneNumber,

                        // Project each associated link into an anonymous type
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
                // Read the request body to get the JSON data
                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    // Deserialize the JSON data into a JsonDocument
                    var json = JsonSerializer.Deserialize<JsonDocument>(requestBody);

                    // Extract the interestId from the JSON data
                    var interestId = json.RootElement.GetProperty("interestId").GetString();

                    // Check if interestId is null or empty
                    if (string.IsNullOrEmpty(interestId))
                    {
                        return Results.BadRequest("InterestId is required");
                    }

                    // Retrieve the person with the specified ID from the database
                    var person = context.Person
                        .Include(p => p.PersonInterestLinks)
                        .Where(p => p.Id == personId)
                        .FirstOrDefault();

                    // Retrieve the interest with the specified ID from the database
                    var interest = context.Interest
                        .Where(i => i.Id == Convert.ToInt32(interestId))
                        .FirstOrDefault();

                    // Check if person or interest is null
                    if (person == null || interest == null)
                    {
                        return Results.BadRequest("Person or Interest not found");
                    }

                    // Create a new PersonInterestLink instance
                    var personInterestLink = new PersonInterestLink
                    {
                        Person = person,
                        Interest = interest
                    };

                    // Add the PersonInterestLink to the context and save changes to the database
                    context.PersonInterestLink.Add(personInterestLink);
                    context.SaveChanges();

                    // Return Ok result if the operation is successful
                    return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                // Return BadRequest result if an exception occurs during the process
                return Results.BadRequest("Invalid request.");
            }
        }


        public static async Task<IResult> addLinksToPersonInterest(HttpContext httpContext, ApplicationContext context, int personId, int interestId)
        {
            try
            {
                // Read the request body to get the JSON data
                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    // Deserialize the JSON data into a JsonDocument
                    var json = JsonSerializer.Deserialize<JsonDocument>(requestBody);

                    // Extract the array of links from the JSON data
                    var linksArray = json.RootElement.GetProperty("links").EnumerateArray();

                    // Retrieve the person with the specified ID from the database
                    var person = context.Person
                        .Include(p => p.PersonInterestLinks)
                        .FirstOrDefault(p => p.Id == personId);

                    // Retrieve the interest with the specified ID from the database
                    var interest = context.Interest
                        .Include(i => i.PersonInterestLinks)
                        .FirstOrDefault(i => i.Id == interestId);

                    // Check if person or interest is null
                    if (person == null || interest == null)
                    {
                        return Results.BadRequest("Person or Interest not found");
                    }

                    // Iterate through each link in the links array
                    foreach (var linkElement in linksArray)
                    {
                        // Extract the URL of the link from the JSON data
                        var linkUrl = linkElement.GetProperty("url").GetString();

                        // Check if the URL is null or empty
                        if (string.IsNullOrEmpty(linkUrl))
                        {
                            return Results.BadRequest("Link URL is required");
                        }

                        // Create a new Link instance
                        var link = new Link
                        {
                            Url = linkUrl,
                            Person = person,
                            Interest = interest
                        };

                        // Add the link to the context
                        context.Link.Add(link);
                    }

                    // Save changes to the database
                    context.SaveChanges();

                    // Return Ok result if the operation is successful
                    return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                // Return BadRequest result if an exception occurs during the process
                return Results.BadRequest("Invalid request.");
            }
        }


    }
}