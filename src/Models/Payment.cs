using System;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents a user payment record
    /// </summary>
    public class Payment
    {
        public virtual Guid PaymentId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual Guid PayTo { get; set; }
        public virtual PaymentStatuses Status { get; set; }
        public virtual string ClosedReason { get; set; }
        public virtual DateTime Date { get; set; }
    }
}