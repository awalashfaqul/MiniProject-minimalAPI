using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Handlers;
using Microsoft.EntityFrameworkCore;

namespace API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
      builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connectionString));
      var app = builder.Build();

      app.MapGet("/persons", Handler.getPersons);
      app.MapGet("/persons/{personId}/interests", (ApplicationContext context, int personId) => Handler.getInterestsForPerson(context, personId));
      app.MapGet("/persons/{personId}/links", (ApplicationContext context, int personId) => Handler.getLinksForPerson(context, personId));

      app.MapPost("/persons/{personId}/interests", async (ApplicationContext context, int personId, HttpContext httpContext) => await Handler.addInterestToPerson(context, personId, httpContext));
      app.MapPost("/persons/{personId}/interests/{interestId}/links", async (HttpContext httpContext, ApplicationContext context, int personId, int interestId) => await Handler.addLinksToPersonInterest(httpContext, context, personId, interestId));

      app.Run();
    }
  }
}
