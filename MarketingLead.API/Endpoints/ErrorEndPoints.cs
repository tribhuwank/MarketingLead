using MarketingLead.API.Wrappers;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace MarketingLead.API.Endpoints;

public static class ErrorEndPoints
{
    public static WebApplication MapErrorEndPoints(this WebApplication app)
    {
        app.MapGet("/error", (HttpContext context, ILogger<Program> logger) =>
        {

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (contextFeature != null)
            {
                var ex = contextFeature.Error;
                logger.LogError(ex.Message);

            }
            Results.Ok(new Response<object> { Succeeded = false, Message = contextFeature!.Error.Message, Errors = contextFeature!.Error });
        });
        return app;
    }

}
