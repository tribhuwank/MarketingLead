using Microsoft.AspNetCore.Authorization;

namespace MarketingLead.API.Security;

public class AdminRoleRequirement: IAuthorizationRequirement
{
    public AdminRoleRequirement(string role) => Role = role;
    public string Role { get; set; }
}
