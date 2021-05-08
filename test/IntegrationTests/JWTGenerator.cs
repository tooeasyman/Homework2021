using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Tests.IntegrationTests
{
    /// <summary>
    ///     Helper class to generate JWT
    /// </summary>
    public static class JWTGenerator
    {
        public static string GenerateJWToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.UserId.ToString())
            };

            var symmetricSecurityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B1CF4B7DC4C4175B6618DE4F55CA4"));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                "Moula",
                "Moula",
                claims,
                expires: DateTime.UtcNow.AddMinutes(600),
                signingCredentials: signingCredentials);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return jwtToken;
        }
    }
}