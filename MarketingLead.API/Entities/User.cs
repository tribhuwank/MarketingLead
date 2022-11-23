using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Entities
{
    public class User:BaseEntity
    {
        public int Id { get; private set; }
        [Required] public string FirstName { get; set; } =String.Empty;
        [Required] public string LastName { get; set; } =String.Empty;
        [Required] public string ContactInfo { get; set; } = String.Empty;
        [Required] public string Role { get; set; } = string.Empty;
        [Required] public string Password { get; set; } =String.Empty;      

    }
}