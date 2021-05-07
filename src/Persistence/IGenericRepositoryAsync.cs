using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rickie.Homework.ShowcaseApp.Persistence
{
    /// <summary>
    ///     Interface that defines persistence access related methods
    /// </summary>
    /// <typeparam name="T">Entities to access</typeparam>
    public interface IGenericRepositoryAsync<T> where T : class
    {
        /// <summary>
        ///     Get all of the records for entity type <see cref="T" />
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> GetAllAsync();

        /// <summary>
        ///     Record lookup for entity type <see cref="T" />
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);

        /// <summary>
        ///     Get a specific page of records for  entity type <see cref="T" />
        /// </summary>
        /// <param name="pageNumber">Which page to get</param>
        /// <param name="pageSize">How many records to get</param>
        /// <returns></returns>
        Task<IQueryable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        /// <summary>
        ///     Add a record of type <see cref="T" /> to persistence
        /// </summary>
        /// <param name="entity">Record to add</param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        ///     Update a record of type <see cref="T" /> to persistence
        /// </summary>
        /// <param name="entity">Record to update</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        ///     Delete a record of type <see cref="T" /> from persistence
        /// </summary>
        /// <param name="entity">Record to delete</param>
        /// <returns></returns>
        Task DeleteAsync(T entity);
    }
}