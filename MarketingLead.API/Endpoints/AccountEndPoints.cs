using MarketingLead.API.Data;
using MarketingLead.API.Endpoints.Schemas;
using MarketingLead.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace MarketingLead.API.Endpoints;

public static class AccountEndPoints
{
    private const string Tag = "Marketing Module";
    private const string ById = "api/account/byid";
    public static WebApplication MapAccountEndPoints(this WebApplication app)
    {
        app.MapGet("/api/accounts", GetAllAccounts)
           .RequireAuthorization()
           .WithTags(Tag)
           .Produces<ICollection<Account>>(200)
           .Produces(401)
           .WithDisplayName("Get all accounts");

        app.MapGet("/api/accounts/{accountId}", GetAccount)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Account>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an Account by id");

        app.MapDelete("/api/accounts/{accountId}", DeleteAccount)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a Account by id");

        app.MapPut("/api/accounts/{accountId}", UpdateAccount)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<Account>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an Account by id");

        app.MapPost("/api/accounts", CreateAccount)
             .AllowAnonymous()
            .WithTags(Tag)
            .Accepts<Account>("application/json")
            .Produces<Account>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an Account");
        return app;


    }
    private static async Task<IResult> GetAllAccounts(ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var accounts = await db.Accounts
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(accounts.Select(account =>
            new ApiAccount(account.Id,account.Payments, account.DealAmount, account.TotalPayments, account.RemainingBalance, account.PaymentDueDate, account.Status)));
    }

    private static async Task<IResult> GetAccount([FromRoute] int accountId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == accountId);

        if (account == null) return Results.NotFound();

        return Results.Ok(
            new ApiAccount(account.Id, account.Payments, account.DealAmount, account.TotalPayments, account.RemainingBalance, account.PaymentDueDate, account.Status));
    }

    private static async Task<IResult> DeleteAccount([FromRoute] int accountId, ClaimsPrincipal principal, MarketingLeadDb db)
    {

        var account = await db.Accounts
            .FirstOrDefaultAsync(s => s.Id == accountId);

        if (account == null) return Results.NotFound();

        db.Accounts.Remove(account);
        await db.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdateAccount([FromRoute] int accountId, [FromBody] ApiAccount apiAccount, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {

        var account = await db.Accounts
            .FirstOrDefaultAsync(s => s.Id == accountId);

        if (account == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(apiAccount, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        account.Payments = apiAccount.Payments;
        account.DealAmount = apiAccount.DealAmount;
        account.TotalPayments = apiAccount.TotalPayments;
        account.RemainingBalance = apiAccount.RemainingBalance;
        account.PaymentDueDate = apiAccount.PaymentDueDate;
        account.Status = apiAccount.Status;

        await db.SaveChangesAsync();
        return Results.Accepted(
            link.GetUriByName(http, ById, new { accountId = account.Id })!,
            new ApiAccount(account.Id, account.Payments, account.DealAmount, account.TotalPayments, account.RemainingBalance, account.PaymentDueDate, account.Status));
    }

    private static async Task<IResult> CreateAccount([FromBody] ApiAccount apiAccount, ClaimsPrincipal principal, MarketingLeadDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiAccount, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        var account = new Account
        {
            Payments = apiAccount.Payments,
            DealAmount = apiAccount.DealAmount,
            TotalPayments = apiAccount.TotalPayments,
            RemainingBalance = apiAccount.RemainingBalance,
            PaymentDueDate= apiAccount.PaymentDueDate,            
            Status = apiAccount.Status
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return Results.Created(link.GetUriByName(http, ById, new { accountId = account.Id })!,
            new ApiAccount(account.Id, account.Payments, account.DealAmount, account.TotalPayments, account.RemainingBalance, account.PaymentDueDate, account.Status));
    }
}
