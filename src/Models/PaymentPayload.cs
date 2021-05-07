using System;
using Rickie.Homework.ShowcaseApp.Queries;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents result of <see cref="GetPaymentsQuery" />
    /// </summary>
    public class PaymentPayload
    {
        public virtual Guid PaymentId { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual Guid PayTo { get; set; }

        public virtual string Status { get; set; }

        public virtual string ClosedReason { get; set; }

        public virtual DateTime Date { get; set; }
    }
}