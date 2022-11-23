using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiPayment(
 int? Id,
[property: Required] DateOnly PaymentDate,
[property: Required] int AccountId,
[property: Required] decimal Amount,
[property: Required] string Method,
[property: Required] int PaymentCategoryId,
[property: Required] bool Status);
