using MarketingLead.API.Data;
using MarketingLead.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using MarketingLead.API.Endpoints.Schemas;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class UserEndPoints
{
    private const string Tag = "User Module";
    private const string ById = "api/user/byid";
    public static WebApplication MapUserEndPoints(this WebApplication app)
    {
        app.MapGet("/api/users", GetAllUsers)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<User>>(200)
           .Produces(401)
           .WithDisplayName("Get all users");

        app.MapGet("/api/users/{userId}", GetUser)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<User>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an User by id");

        app.MapDelete("/api/users/{userId}", DeleteUser)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a User by id");

        app.MapPut("/api/users/{userId}", UpdateUser)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<User>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an User by id");

        app.MapPost("/api/users", CreateUser)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<User>("application/json")
            .Produces<User>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an User");
        return app;


    }
    private static async Task<IResult> GetAllUsers(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var users = await db.Users
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(users.Select(user =>
            new ApiUser(user.Id, user.FirstName, user.LastName,user.ContactInfo,user.Password,user.Role)));
    }

    private static async Task<IResult> GetUser([FromRoute] int userId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == userId);

        if (user == null) return Results.NotFound();

        return Results.Ok(
            new ApiUser(user.Id, user.FirstName,user.LastName,user.ContactInfo, user.Role, user.Password));
    }

    private static async Task<IResult> DeleteUser([FromRoute] int userId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var user = await db.Users
            .FirstOrDefaultAsync(s => s.Id == userId);

        if (user == null) return Results.NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdateUser([FromRoute] int userId, [FromBody] ApiUser apiUser, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var user = await db.Users
            .FirstOrDefaultAsync(s => s.Id == userId);

        if (user == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiUser, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        user.FirstName = apiUser.FirstName;
        user.LastName = apiUser.LastName;
        user.ContactInfo = apiUser.ContactInfo;
        user.Role = apiUser.Role;
        user.Password = apiUser.Password;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { userId = user.Id })!,
            new ApiUser(user.Id, user.FirstName, user.LastName, user.ContactInfo, user.Role, user.Password));
    }

    private static async Task<IResult> CreateUser([FromBody] ApiUser apiUser, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiUser, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var user = new User
        {
            FirstName = apiUser.FirstName,
            LastName = apiUser.LastName,
            ContactInfo = apiUser.ContactInfo,
            Role = apiUser.Role,
            Password = apiUser.Password
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { userId = user.Id })!,
            new ApiUser(user.Id, user.FirstName, user.LastName, user.ContactInfo, user.Role, user.Password));
    }
}
