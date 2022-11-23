using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiUser(

     int? Id,
    [property: Required] string FirstName,
    [property: Required] string LastName,
    [property: Required] string ContactInfo,
    [property: Required] string Role,
    [property: Required] string Password);