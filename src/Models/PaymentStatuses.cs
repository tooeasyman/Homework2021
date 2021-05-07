namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents payment status
    /// </summary>
    public class PaymentStatuses
    {
        public virtual int PaymentStatusId { get; set; }
        public virtual string StatusDescription { get; set; }
        public virtual string StatusValue { get; set; }
    }
}