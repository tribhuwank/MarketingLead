using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Entities;

public class Account:BaseEntity
{
    public int Id { get; set; }
    [Required] public string Payments { get; set; }=string.Empty;
    [Required][Precision(14, 2)] public decimal DealAmount { get; set; }
    [Required][Precision(14, 2)] public decimal TotalPayments { get; set; }
    [Required][Precision(14, 2)] public decimal RemainingBalance { get; set;}
    [Required] public DateOnly PaymentDueDate { get; set; }
    [Required] public bool Status { get; set; }

}
