
using Microsoft.OpenApi.Models;
using MarketingLead.API.Middlewares;

namespace MarketingLead.API.Startup;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder AddSwaggerServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "MarketingLeadDb API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Cagtu",
                    Email = "tribhuwankushwaha@cagtu.com"
                }
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
    
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }});
    
           
        });

        return builder;
    }
}