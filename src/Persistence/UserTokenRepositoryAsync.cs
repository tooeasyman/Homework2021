using NHibernate;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Entity type <see cref="UserToken" /> related persistence access
    /// </summary>
    public class UserTokenRepositoryAsync : GenericRepositoryAsync<UserToken>, IUserTokenRepositoryAsync
    {
        public UserTokenRepositoryAsync(ISession session) : base(session)
        {
        }
    }
}