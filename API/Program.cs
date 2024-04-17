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
            // Create a new web application builder instance
            var builder = WebApplication.CreateBuilder(args);

            // Retrieve the connection string from the configuration
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add the DbContext service to the service collection, using SQL Server as the database provider
            builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connectionString));

            // Build the web application
            var app = builder.Build();

            // Map a GET request to "/persons" endpoint to the getPersons handler method
            app.MapGet("/persons", Handler.getPersons);

            // Map a GET request to "/persons/{personId}/interests" endpoint to the getInterestsForPerson handler method
            app.MapGet("/persons/{personId}/interests", (ApplicationContext context, int personId) => Handler.getInterestsForPerson(context, personId));

            // Map a GET request to "/persons/{personId}/links" endpoint to the getLinksForPerson handler method
            app.MapGet("/persons/{personId}/links", (ApplicationContext context, int personId) => Handler.getLinksForPerson(context, personId));

            // Map a POST request to "/persons/{personId}/interests" endpoint to the addInterestToPerson handler method
            app.MapPost("/persons/{personId}/interests", async (ApplicationContext context, int personId, HttpContext httpContext) => await Handler.addInterestToPerson(context, personId, httpContext));

            // Map a POST request to "/persons/{personId}/interests/{interestId}/links" endpoint to the addLinksToPersonInterest handler method
            app.MapPost("/persons/{personId}/interests/{interestId}/links", async (HttpContext httpContext, ApplicationContext context, int personId, int interestId) => await Handler.addLinksToPersonInterest(httpContext, context, personId, interestId));

            // Run the application
            app.Run();
        }
    }
}
