using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiToken(
    string AccessToken,
    [property: Required] string RefreshToken);