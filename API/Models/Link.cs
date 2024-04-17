

namespace API.Models
{
  
    public class Link
    {
        // Property to store the unique identifier of the link
        public int Id { get; set; }

        // Property to store the URL of the link
        public string Url { get; set; }

        // Navigation property representing the person associated with this link
        public virtual Person Person { get; set; }

        // Navigation property representing the interest associated with this link
        public virtual Interest Interest { get; set; }
    }

}