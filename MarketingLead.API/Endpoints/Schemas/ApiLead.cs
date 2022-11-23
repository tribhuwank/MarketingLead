using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiLead(
     int? Id,
    [property: Required] string Name,
    [property: Required] int ClientManagerId,
    [property:Required] int AccountId,
    [property: Required] bool Status);
