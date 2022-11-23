using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Entities;

public class PaymentCategory:BaseEntity
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
}
