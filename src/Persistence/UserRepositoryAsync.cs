using NHibernate;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Entity type <see cref="User" /> related persistence access
    /// </summary>
    public class UserRepositoryAsync : GenericRepositoryAsync<User>, IUserRepositoryAsync
    {
        public UserRepositoryAsync(ISession session) : base(session)
        {
        }
    }
}