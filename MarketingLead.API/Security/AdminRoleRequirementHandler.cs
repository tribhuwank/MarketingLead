using Microsoft.AspNetCore.Authorization;

namespace MarketingLead.API.Security
{
    public class AdminRoleRequirementHandler:AuthorizationHandler<AdminRoleRequirement>
    {
        public AdminRoleRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Value == requirement.Role))
            {
                context.Succeed(requirement);
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                await _httpContextAccessor.HttpContext.Response.WriteAsJsonAsync(new { StatusCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized. Required admin role." });
                await _httpContextAccessor.HttpContext.Response.CompleteAsync();
                context.Fail();

            }

        }        

        private readonly IHttpContextAccessor _httpContextAccessor;
    }
}
