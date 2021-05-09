using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Middlewares
{
    /// <summary>
    ///     Middleware to attach ID of the user represented by current bearer token to http context
    /// </summary>
    public class JwtMiddleware
    {
        private readonly JwtConfiguration _jwtSettings;
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtConfiguration> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private async void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "uid").Value);

                context.Items["UserId"] = userId;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user id is not attached to context so request won't have access to secure routes
            }
        }
    }
}