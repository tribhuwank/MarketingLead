using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Endpoints.Schemas;

public record ApiLoginUser(
    [property:Required]string UserName,
    [property:Required]string Password);