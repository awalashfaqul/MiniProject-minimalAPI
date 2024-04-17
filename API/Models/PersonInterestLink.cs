

namespace API.Models
{
    public class PersonInterestLink
    {
        // Property to store the unique identifier of the link between a person and an interest
        public int Id { get; set; }

        // Navigation property representing the person associated with this link
        public virtual Person Person { get; set; }

        // Navigation property representing the interest associated with this link
        public virtual Interest Interest { get; set; }
    }

}