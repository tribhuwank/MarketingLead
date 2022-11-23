
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MarketingLead.API.Endpoints;
using MarketingLead.API.Middlewares;
using MarketingLead.API.Startup;
using MarketingLead.API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSqlServer<MarketingLeadDb>(@"Data Source=127.0.0.1,1433;encrypt=false; Initial Catalog=MarketingDb;User Id=SA;Password=Admin@2022;");
builder.Services.AddSqlServer<MarketingLeadDb>(@"Data Source=host.docker.internal,1433; Initial Catalog=db;User Id=sa;Password=Admin@2022;TrustServerCertificate=True;MultiSubnetFailover=True");

builder.AddAuthenticationServices();
builder.AddSwaggerServices();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHealthChecks("/health");
await EnsureDb(app.Services, app.Logger);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapSwaggerEndpoints();
app.MapAuthenticationEndpoints();
app.MapUserEndPoints();
app.MapPaymentCategoryEndPoints();
app.MapTeamEndPoints();
app.MapLeadEndPoints();
app.MapPaymentEndPoints();
app.MapAccountEndPoints();




app.UseMiddleware<JwtMiddleware>(new TokenValidationParameters
{
    ValidateActor = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Issuer"],
    ValidAudience = builder.Configuration["Audience"],
    IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]))
});

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    await using var db = services.CreateScope().ServiceProvider.GetRequiredService<MarketingLeadDb>();
    if (db.Database.IsRelational())
    {
        logger.LogInformation("Updating database...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Updated database");
    }
}