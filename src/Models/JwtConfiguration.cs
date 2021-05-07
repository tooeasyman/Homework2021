namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     JWT configuration options
    /// </summary>
    public class JwtConfiguration
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }
        public double TokenLifetimeInMin { get; set; }
    }
}