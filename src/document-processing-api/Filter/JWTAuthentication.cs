using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace DocumentProcessing.API.Filter
{
    /// <summary>
    /// Class that represents the JWT authentication filter.
    /// </summary>
    public class JWTAuthentication : ActionFilterAttribute
    {
        /// <summary>
        /// Method that is called before the action is executed.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var enableSecurity = Environment.GetEnvironmentVariable("ENABLE_JWT_SECURITY");

            if (string.IsNullOrEmpty(enableSecurity) || !bool.TryParse(enableSecurity, out var authorizeValue) || !authorizeValue)
            {
                // Authorization is disabled, proceed without validation
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Result = new UnauthorizedObjectResult("Authorization token is missing.");
                return;
            }

            var tokenValue = token.ToString().Replace("Bearer ", "");

            string? publicKey = Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY");

            if (string.IsNullOrEmpty(publicKey))
            {
                context.Result = new UnauthorizedObjectResult("JWT_PUBLIC_KEY is missing.");
                return;
            }

            if (!ValidateToken(tokenValue, publicKey, out var errorMessage))
            {
                context.Result = new UnauthorizedObjectResult(errorMessage);
                return;
            }

            await next();
        }

        private bool ValidateToken(string token, string publicKey, out string? errorMessage)
        {

            errorMessage = null;

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Validate token
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenExpiredException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (SecurityTokenInvalidLifetimeException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (SecurityTokenValidationException ex)
            {
                errorMessage = "Token validation failed: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred during token validation. Please try again later." + ex.Message;
                return false;
            }
        }
    }
}
