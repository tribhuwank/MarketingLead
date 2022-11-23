using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiPaymentCategory(
     int? Id,
    [property: Required] string Name);
