using System.ComponentModel.DataAnnotations;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents a client authentication request
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// User name
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        ///     Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}