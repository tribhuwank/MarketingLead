using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Entities;

public class Payment:BaseEntity
{
    public int Id { get; set; }
    [Required] public DateOnly PaymentDate { get; set; }
    [Required] public int AccountId { get; set; }
    public Account Account { get; set; }=new Account();
    [Required][Precision(14, 2)] public decimal Amount { get; set; }
    [Required] public string Method { get; set; } = string.Empty;
    [Required] public int PaymentCategoryId { get; set; }
    public PaymentCategory PaymentCategory { get; set; }=new PaymentCategory();
    [Required] public bool Status { get; set; }
}
