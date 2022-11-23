using MarketingLead.API.Data;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class PaymentEndPoints
{
    private const string Tag = "Marketing Module";
    private const string ById = "api/payment/byid";
    public static WebApplication MapPaymentEndPoints(this WebApplication app)
    {
        app.MapGet("/api/payments", GetAllPayments)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<Payment>>(200)
           .Produces(401)
           .WithDisplayName("Get all payments");

        app.MapGet("/api/payments/{paymentId}", GetPayment)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Payment>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an Payment by id");

        app.MapDelete("/api/payments/{paymentId}", DeletePayment)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a Payment by id");

        app.MapPut("/api/payments/{paymentId}", UpdatePayment)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Payment>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an Payment by id");

        app.MapPost("/api/payments", CreatePayment)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<Payment>("application/json")
            .Produces<Payment>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an Payment");
        return app;


    }
    private static async Task<IResult> GetAllPayments(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var payments = await db.Payments
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(payments.Select(payment =>
            new ApiPayment(payment.Id, payment.PaymentDate, payment.AccountId,payment.Amount,payment.Method,payment.PaymentCategoryId, payment.Status)));
    }

    private static async Task<IResult> GetPayment([FromRoute] int paymentId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var payment = await db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == paymentId);

        if (payment == null) return Results.NotFound();

        return Results.Ok(
            new ApiPayment(payment.Id, payment.PaymentDate, payment.AccountId, payment.Amount, payment.Method, payment.PaymentCategoryId, payment.Status));
    }

    private static async Task<IResult> DeletePayment([FromRoute] int paymentId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var payment = await db.Payments
            .FirstOrDefaultAsync(s => s.Id == paymentId);

        if (payment == null) return Results.NotFound();

        db.Payments.Remove(payment);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdatePayment([FromRoute] int paymentId, [FromBody] ApiPayment apiPayment, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var payment = await db.Payments
            .FirstOrDefaultAsync(s => s.Id == paymentId);

        if (payment == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiPayment, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        payment.PaymentDate = apiPayment.PaymentDate;
        payment.AccountId = apiPayment.AccountId;
        payment.Amount = apiPayment.Amount;
        payment.Method = apiPayment.Method;
        payment.PaymentCategoryId = apiPayment.PaymentCategoryId;
        payment.Status = apiPayment.Status;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { paymentId = payment.Id })!,
            new ApiPayment(payment.Id, payment.PaymentDate, payment.AccountId, payment.Amount, payment.Method, payment.PaymentCategoryId, payment.Status));
    }

    private static async Task<IResult> CreatePayment([FromBody] ApiPayment apiPayment, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiPayment, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var payment = new Payment
        {
            PaymentDate = apiPayment.PaymentDate,
            AccountId = apiPayment.AccountId,
            Method = apiPayment.Method,
            PaymentCategoryId = apiPayment.PaymentCategoryId,
            Status = apiPayment.Status
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { paymentId = payment.Id })!,
            new ApiPayment(payment.Id, payment.PaymentDate, payment.AccountId, payment.Amount, payment.Method, payment.PaymentCategoryId, payment.Status));
    }
}
