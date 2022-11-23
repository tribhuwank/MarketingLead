using MarketingLead.API.Data;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class TeamEndpoints
{
    private const string Tag = "Marketing Module";
    private const string ById = "api/team/byid";
    public static WebApplication MapTeamEndPoints(this WebApplication app)
    {
        app.MapGet("/api/teams", GetAllTeams)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<Team>>(200)
           .Produces(401)
           .WithDisplayName("Get all teams");

        app.MapGet("/api/teams/{teamId}", GetTeam)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Team>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an Team by id");

        app.MapDelete("/api/teams/{teamId}", DeleteTeam)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a Team by id");

        app.MapPut("/api/teams/{teamId}", UpdateTeam)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Team>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an Team by id");

        app.MapPost("/api/teams", CreateTeam)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<Team>("application/json")
            .Produces<Team>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an Team");
        return app;


    }
    private static async Task<IResult> GetAllTeams(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var teams = await db.Teams
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(teams.Select(team =>
            new ApiTeam(team.Id, team.Name)));
    }

    private static async Task<IResult> GetTeam([FromRoute] int teamId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var team = await db.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == teamId);

        if (team == null) return Results.NotFound();

        return Results.Ok(
            new ApiTeam(team.Id, team.Name));
    }

    private static async Task<IResult> DeleteTeam([FromRoute] int teamId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var team = await db.Teams
            .FirstOrDefaultAsync(s => s.Id == teamId);

        if (team == null) return Results.NotFound();

        db.Teams.Remove(team);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdateTeam([FromRoute] int teamId, [FromBody] ApiTeam apiTeam, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var team = await db.Teams
            .FirstOrDefaultAsync(s => s.Id == teamId);

        if (team == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiTeam, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        team.Name = apiTeam.Name;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { teamId = team.Id })!,
            new ApiTeam(team.Id, team.Name));
    }

    private static async Task<IResult> CreateTeam([FromBody] ApiTeam apiTeam, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiTeam, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var team = new Team
        {
            Name = apiTeam.Name
        };
        db.Teams.Add(team);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { teamId = team.Id })!,
            new ApiTeam(team.Id, team.Name));
    }
}
