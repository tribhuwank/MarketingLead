using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiAccount(
     int? Id,
    [property: Required] string Payments,
    [property: Required] decimal DealAmount,
    [property: Required] decimal TotalPayments,
    [property: Required] decimal RemainingBalance,
    [property: Required] DateOnly PaymentDueDate,
    [property: Required] bool Status);
