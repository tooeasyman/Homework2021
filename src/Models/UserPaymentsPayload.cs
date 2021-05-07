using System;
using System.Collections.Generic;
using Rickie.Homework.ShowcaseApp.Queries;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents result of <see cref="GetPaymentsQuery" />
    /// </summary>
    public class UserPaymentsPayload
    {
        public virtual Guid UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual IEnumerable<PaymentPayload> Payments { get; set; }
    }
}