using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MarketingLead.API.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _tokenValidationParams;
        public JwtMiddleware(RequestDelegate next, TokenValidationParameters
        tokenValidationParams)
        {
            _next = next;
            _tokenValidationParams = tokenValidationParams;
        }
        

        public async Task Invoke(HttpContext context)
        {

            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenInVerification = jwtTokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {                       
                        context.Items["Error"] = new
                        {
                            Success = false,
                            Errors = "Token is Invalid"
                        };
                    }
                    else
                    {
                        var sub=jwtSecurityToken.Subject;
                        context.Items["CurrentUser"] = sub;
                    }
                }
            }
            catch (Exception)
            {
                context.Items["Error"] = new
                {
                    Success = false,
                    Errors = "Token does not match or may expired."
                };
            } 
            await _next(context);
        }
       
    }
}
