using MarketingLead.API.Data;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MiniValidation;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class LeadEndPoints
{
    private const string Tag = "Marketing Module";
    private const string ById = "api/lead/byid";
    public static WebApplication MapLeadEndPoints(this WebApplication app)
    {
        app.MapGet("/api/leads", GetAllLeads)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<Lead>>(200)
           .Produces(401)
           .WithDisplayName("Get all leads");

        app.MapGet("/api/leads/{leadId}", GetLead)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Lead>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an Lead by id");

        app.MapDelete("/api/leads/{leadId}", DeleteLead)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a Lead by id");

        app.MapPut("/api/leads/{leadId}", UpdateLead)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Lead>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an Lead by id");

        app.MapPost("/api/leads", CreateLead)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<Lead>("application/json")
            .Produces<Lead>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an Lead");
        return app;


    }
    private static async Task<IResult> GetAllLeads(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var leads = await db.Leads
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(leads.Select(lead =>
            new ApiLead(lead.Id, lead.Name, lead.ClientManagerId, lead.AccountId, lead.Status)));
    }

    private static async Task<IResult> GetLead([FromRoute] int leadId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var lead = await db.Leads
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == leadId);

        if (lead == null) return Results.NotFound();

        return Results.Ok(
            new ApiLead(lead.Id, lead.Name, lead.ClientManagerId, lead.AccountId, lead.Status));
    }

    private static async Task<IResult> DeleteLead([FromRoute] int leadId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var lead = await db.Leads
            .FirstOrDefaultAsync(s => s.Id == leadId);

        if (lead == null) return Results.NotFound();

        db.Leads.Remove(lead);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdateLead([FromRoute] int leadId, [FromBody] ApiLead apiLead, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var lead = await db.Leads
            .FirstOrDefaultAsync(s => s.Id == leadId);

        if (lead == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiLead, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        lead.Name = apiLead.Name;
        lead.ClientManagerId = apiLead.ClientManagerId;
        lead.AccountId = apiLead.AccountId;
        lead.Status= apiLead.Status;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { leadId = lead.Id })!,
            new ApiLead(lead.Id, lead.Name, lead.ClientManagerId, lead.AccountId, lead.Status));
    }

    private static async Task<IResult> CreateLead([FromBody] ApiLead apiLead, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiLead, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var lead = new Lead
        {
            Name = apiLead.Name,
            ClientManagerId = apiLead.ClientManagerId,
            AccountId = apiLead.AccountId,
            Status = apiLead.Status
        };
        db.Leads.Add(lead);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { leadId = lead.Id })!,
            new ApiLead(lead.Id, lead.Name, lead.ClientManagerId, lead.AccountId, lead.Status));
    }
}
