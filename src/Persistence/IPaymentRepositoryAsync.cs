using Rickie.Homework.ShowcaseApp.Models;
using System.Threading.Tasks;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Interface for <see cref="Payment" /> persistence access
    /// </summary>
    public interface IPaymentRepositoryAsync : IGenericRepositoryAsync<Payment>
    {
        /// <summary>
        ///     Make a payment
        /// </summary>
        /// <param name="entity">Payment details</param>
        /// <returns></returns>
        Task<Payment> MakePaymentAsync(Payment entity);
    }
}