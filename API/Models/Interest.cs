

namespace API.Models
{
  
    public class Interest
    {
        // Property to store the unique identifier of the interest
        public int Id { get; set; }

        // Property to store the title of the interest
        public string Title { get; set; }

        // Property to store the description of the interest
        public string Description { get; set; }

        // Navigation property representing the collection of links between persons and this interest
        public virtual ICollection<PersonInterestLink> PersonInterestLinks { get; set; }

        // Navigation property representing the collection of links associated with this interest
        public virtual ICollection<Link> Links { get; set; }
    }

}