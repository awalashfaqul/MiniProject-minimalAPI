

namespace API.Models
{
    public class Person
    {
        // Property to store the unique identifier of the person
        public int Id { get; set; }

        // Property to store the first name of the person
        public string FirstName { get; set; }

        // Property to store the last name of the person
        public string LastName { get; set; }

        // Property to store the phone number of the person
        public string PhoneNumber { get; set; }

        // Navigation property representing the collection of links between this person and interests
        public virtual ICollection<PersonInterestLink> PersonInterestLinks { get; set; }

        // Navigation property representing the collection of links associated with this person
        public virtual ICollection<Link> Links { get; set; }
    }

}