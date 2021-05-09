using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Services;

namespace Rickie.Homework.ShowcaseApp.Controllers
{
    /// <summary>
    ///     Controller that takes care of authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private static readonly string LocalIp = "127.0.0.1";
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        ///     Authenticate a user account
        /// </summary>
        /// <param name="request">Authentication request payload</param>
        /// <returns>JWT if authentication succeeds, error reason otherwise</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _authenticationService.AuthenticateAsync(request, GetRequestIpAddress()));
        }

        private string GetRequestIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            if (HttpContext.Connection.RemoteIpAddress != null)
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return LocalIp;
        }
    }
}