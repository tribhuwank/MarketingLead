using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniValidation;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using MarketingLead.API.Data;

namespace MarketingLead.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/login", async (ApiLoginUser apiUser, MarketingLeadDb db, IConfiguration configuration) =>
        {
            if (!MiniValidator.TryValidate(apiUser, out _))
            {
                return Results.Unauthorized();
            }

            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInfo == apiUser.UserName);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            string accessToken = GenerateAccessToken(configuration, user);
            string refreshToken = GenerateRefreshToken(configuration, user);
            return Results.Ok(
                new ApiToken(AccessToken: accessToken, RefreshToken: refreshToken));
        })
            .AllowAnonymous()
            .WithTags("Authentication")
            .Produces(200)
            .Produces(401);
        app.MapPost("/token/refresh", (ApiToken apiToken, MarketingLeadDb db, IConfiguration configuration) =>
        {
            JwtSecurityToken secToken = new JwtSecurityTokenHandler().ReadJwtToken(apiToken.RefreshToken);

            var user = db.Users.FirstOrDefault(u => u.ContactInfo == secToken.Subject);
            if (user == null)
            {
                return Results.Unauthorized();
            }
            string accessToken = GenerateAccessToken(configuration, user);
            string refreshToken = GenerateRefreshToken(configuration, user);

            return Results.Ok(
                new ApiToken(AccessToken: accessToken, RefreshToken: refreshToken));
        })
           .AllowAnonymous()
           .WithTags("Authentication")
           .Produces(200)
           .Produces(401);
       

        return app;
    }

    private static string GenerateAccessToken(IConfiguration configuration, User user)
    {
        var claims = new[]
        {
                    new Claim(JwtRegisteredClaimNames.Typ, "access"),
                    new Claim(JwtRegisteredClaimNames.Sub, user.ContactInfo)

        };
        var token = new JwtSecurityToken
        (
            issuer: configuration["Issuer"],
            audience: configuration["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SigningKey"])),
                SecurityAlgorithms.HmacSha256)
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return accessToken;
    }

    private static string GenerateRefreshToken(IConfiguration configuration, User user)
    {
        var claims = new[]
        {
           new Claim(JwtRegisteredClaimNames.Typ, "refresh"),
           new Claim(JwtRegisteredClaimNames.Sub, user.ContactInfo)
        };


        var refresh = new JwtSecurityToken
        (
            issuer: configuration["Issuer"],
            audience: configuration["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(120),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SigningKey"])),
                SecurityAlgorithms.HmacSha256)
        );
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(refresh);
        return refreshToken;
    }
}