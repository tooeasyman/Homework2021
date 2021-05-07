using System;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents user account balance
    /// </summary>
    public class UserBalance
    {
        public virtual Guid UserBalanceId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual Decimal Balance { get; set; }
    }
}
