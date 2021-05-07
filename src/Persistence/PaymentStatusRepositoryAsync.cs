using NHibernate;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Entity type <see cref="PaymentStatuses" /> related persistence access
    /// </summary>
    public class PaymentStatusRepositoryAsync : GenericRepositoryAsync<PaymentStatuses>, IPaymentStatusRepositoryAsync
    {
        public PaymentStatusRepositoryAsync(ISession session) : base(session)
        { }
    }
}
