using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiTeam(
 int? Id,
[property: Required] string Name);
