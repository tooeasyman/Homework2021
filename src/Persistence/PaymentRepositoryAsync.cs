using System.Threading.Tasks;
using NHibernate;
using NHibernate.Transform;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Entity type <see cref="Payment" /> related persistence access
    /// </summary>
    public class PaymentRepositoryAsync : GenericRepositoryAsync<Payment>, IPaymentRepositoryAsync
    {
        public PaymentRepositoryAsync(ISession session) : base(session)
        {
        }

        /// <summary>
        ///     Make a payment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Payment> MakePaymentAsync(Payment entity)
        {
            return Session.GetNamedQuery("MakePayment").SetGuid("UserID", entity.UserId)
                .SetDecimal("Amount", entity.Amount)
                .SetGuid("PayTo", entity.PayTo)
                .SetResultTransformer(
                    Transformers.AliasToBean(typeof(Payment))).UniqueResult<Payment>();
        }
    }
}