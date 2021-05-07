using System;
using System.Collections.Generic;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents a user account in the system
    /// </summary>
    public class User
    {
        public User()
        {
            UserTokens = new List<UserToken>();
            Payments = new List<Payment>();
        }

        public virtual Guid UserId { get; set; }

        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual IList<UserToken> UserTokens { get; set; }
        public virtual IList<Payment> Payments { get; set; }
    }
}