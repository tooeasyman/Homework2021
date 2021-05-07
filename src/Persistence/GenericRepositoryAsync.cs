using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Base class for persistence access related classes
    /// </summary>
    /// <typeparam name="T">Entities to access</typeparam>
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        /// <summary>
        ///     NHibernate session
        /// </summary>
        protected readonly ISession Session;

        public GenericRepositoryAsync(ISession session)
        {
            Session = session;
        }


        /// <summary>
        ///     Record lookup for entity type <see cref="T" />
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return (await Session.Query<T>()
                .Where(expression).ToListAsync().ConfigureAwait(false)).AsQueryable();
        }

        /// <summary>
        ///     Get all of the records for entity type <see cref="T" />
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            return (await Session.Query<T>().ToListAsync().ConfigureAwait(false)).AsQueryable();
        }

        /// <summary>
        ///     Get a specific page of records for  entity type <see cref="T" />
        /// </summary>
        /// <param name="pageNumber">Which page to get</param>
        /// <param name="pageSize">How many records to get</param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return (await Session.Query<T>().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
                .ConfigureAwait(false)).AsQueryable();
        }

        /// <summary>
        ///     Add a record of type <see cref="T" /> to persistence
        /// </summary>
        /// <param name="entity">Record to add</param>
        /// <returns></returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Update a record of type <see cref="T" /> to persistence
        /// </summary>
        /// <param name="entity">Record to update</param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            await Session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
            await Session.FlushAsync().ConfigureAwait(false);
            return entity;
        }

        /// <summary>
        ///     Delete a record of type <see cref="T" /> from persistence
        /// </summary>
        /// <param name="entity">Record to delete</param>
        /// <returns></returns>
        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}