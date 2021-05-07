using NHibernate;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Entity type <see cref="UserBalance" /> related persistence access
    /// </summary>
    public class UserBalanceRepositoryAsync : GenericRepositoryAsync<UserBalance>, IUserBalanceRepositoryAsync
    {
        public UserBalanceRepositoryAsync(ISession session) : base(session)
        {
        }
    }
}