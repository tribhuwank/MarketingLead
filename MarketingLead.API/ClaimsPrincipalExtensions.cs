using System.Security.Claims;

namespace MarketingLead.API;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaimValue(this ClaimsPrincipal principal, string type) 
        => principal.FindFirst(type)!.Value;
}