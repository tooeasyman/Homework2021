namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Authentication response
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        ///     User ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     JWT
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}