using System.Threading.Tasks;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Services
{
    /// <summary>
    ///     Interface for user authentication
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
    }
}