using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class ApplicationContext : DbContext
  {
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Person> Person { get; set; }
    public DbSet<Interest> Interest { get; set; }
    public DbSet<Link> Link { get; set; }
    public DbSet<PersonInterestLink> PersonInterestLink { get; set; }
  }
}