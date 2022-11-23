using MarketingLead.API.Data;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class PaymentCategoryEndPoints
{
    private const string Tag = "Marketing Module";
    private const string ById = "api/paymentCategory/byid";
    public static WebApplication MapPaymentCategoryEndPoints(this WebApplication app)
    {
        app.MapGet("/api/paymentCategorys", GetAllPaymentCategorys)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<PaymentCategory>>(200)
           .Produces(401)
           .WithDisplayName("Get all paymentCategorys");

        app.MapGet("/api/paymentCategorys/{paymentCategoryId}", GetPaymentCategory)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<PaymentCategory>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an PaymentCategory by id");

        app.MapDelete("/api/paymentCategorys/{paymentCategoryId}", DeletePaymentCategory)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a PaymentCategory by id");

        app.MapPut("/api/paymentCategorys/{paymentCategoryId}", UpdatePaymentCategory)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<PaymentCategory>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an PaymentCategory by id");

        app.MapPost("/api/PaymentCategory", CreatePaymentCategory)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<PaymentCategory>("application/json")
            .Produces<PaymentCategory>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an PaymentCategory");
        return app;


    }
    private static async Task<IResult> GetAllPaymentCategorys(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var paymentCategorys = await db.PaymentCategorys
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(paymentCategorys.Select(paymentCategory =>
            new ApiPaymentCategory(paymentCategory.Id, paymentCategory.Name)));
    }

    private static async Task<IResult> GetPaymentCategory([FromRoute] int paymentCategoryId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var paymentCategory = await db.PaymentCategorys
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == paymentCategoryId);

        if (paymentCategory == null) return Results.NotFound();

        return Results.Ok(
            new ApiPaymentCategory(paymentCategory.Id, paymentCategory.Name));
    }

    private static async Task<IResult> DeletePaymentCategory([FromRoute] int paymentCategoryId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var paymentCategory = await db.PaymentCategorys
            .FirstOrDefaultAsync(s => s.Id == paymentCategoryId);

        if (paymentCategory == null) return Results.NotFound();

        db.PaymentCategorys.Remove(paymentCategory);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdatePaymentCategory([FromRoute] int paymentCategoryId, [FromBody] ApiPaymentCategory apiPaymentCategory, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var paymentCategory = await db.PaymentCategorys
            .FirstOrDefaultAsync(s => s.Id == paymentCategoryId);

        if (paymentCategory == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiPaymentCategory, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        paymentCategory.Name = apiPaymentCategory.Name;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { paymentCategoryId = paymentCategory.Id })!,
            new ApiPaymentCategory(paymentCategory.Id, paymentCategory.Name));
    }

    private static async Task<IResult> CreatePaymentCategory([FromBody] ApiPaymentCategory apiPaymentCategory, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiPaymentCategory, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var paymentCategory = new PaymentCategory
        {
            Name = apiPaymentCategory.Name
        };
        db.PaymentCategorys.Add(paymentCategory);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { paymentCategoryId = paymentCategory.Id })!,
            new ApiPaymentCategory(paymentCategory.Id, paymentCategory.Name));
    }
}
