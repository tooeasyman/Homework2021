using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rickie.Homework.ShowcaseApp.CustomExceptions;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;

namespace Rickie.Homework.ShowcaseApp.Services
{
    /// <summary>
    ///     Authentication service
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtConfiguration _jwtSettings;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IUserTokenRepositoryAsync _userTokenRepository;

        public AuthenticationService(IUserRepositoryAsync userRepository, IOptions<JwtConfiguration> jwtSettings,
            IPasswordService passwordService, IUserTokenRepositoryAsync userTokenRepository)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _passwordService = passwordService;
            _userTokenRepository = userTokenRepository;
        }

        /// <summary>
        ///     Authenticate a user and issue token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public virtual async Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request,
            string ipAddress)
        {
            IList<ValidationFailure> errorMessages = new List<ValidationFailure>();

            // Identify user by name
            var user = (await _userRepository.FindByCondition(x => x.UserName.ToLower() == request.UserName.ToLower())
                .ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            if (user == null)
                errorMessages.Add(new ValidationFailure("UserName",
                    $"No Accounts Registered with {request.UserName}."));

            // Basic check for password encoding
            var isBase64String = _passwordService.IsBase64String(request.Password);
            if (!isBase64String)
                errorMessages.Add(new ValidationFailure("Password", "Password is not base64 encoded."));

            // Password comparison by hash
            if (user != null)
            {
                var isPasswordValid = _passwordService.VerifyPasswordHash(
                    _passwordService.Base64Decode(request.Password), Convert.FromBase64String(user.PasswordHash),
                    Convert.FromBase64String(user.PasswordSalt));
                if (!isPasswordValid) errorMessages.Add(new ValidationFailure("Password", "Password is Incorrect."));
            }

            if (errorMessages.Count > 0) throw new ValidationException(errorMessages);

            // Generate JWT for future API calls to use
            var jwtSecurityToken = await GenerateJWToken(user);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var userToken = GenerateRefreshToken(ipAddress);
            userToken.JwtToken = jwtToken;
            userToken.UserId = user.UserId;

            user.UserTokens.Add(userToken);
            await _userRepository.UpdateAsync(user).ConfigureAwait(false);

            var response = new AuthenticationResponse
            {
                Id = user.UserId.ToString(),
                Token = jwtToken,
                UserName = user.UserName,
                RefreshToken = userToken.Token
            };

            return new ApiResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }


        private Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.UserId.ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return Task.FromResult(jwtSecurityToken);
        }

        private UserToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new UserToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeInMin),
                    CreatedDate = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}