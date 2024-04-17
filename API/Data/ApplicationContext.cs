using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationContext : DbContext
    {
        // Constructor to initialize the DbContext with options
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        // DbSet for the Person entity
        public DbSet<Person> Person { get; set; }

        // DbSet for the Interest entity
        public DbSet<Interest> Interest { get; set; }

        // DbSet for the Link entity
        public DbSet<Link> Link { get; set; }

        // DbSet for the PersonInterestLink entity, representing the collection of links between persons and interests in the database
        public DbSet<PersonInterestLink> PersonInterestLink { get; set; }
    }

}