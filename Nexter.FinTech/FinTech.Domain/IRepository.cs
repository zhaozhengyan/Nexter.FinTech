using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinTech.Domain;

namespace Nexter.Domain
{
	public interface IRepository : IUnitOfWork
    {
        Task<bool> ExistAsync<T>(Expression<Func<T, bool>> expression) where T : class;
        Task<T> GetByIdAsync<T>(object id, bool throwExceptionIfNotExists = true) where T : class;
        /// <summary>
        /// 当主键由非数据库生成时用此方法Add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Add<T>(T item) where T : class;
        /// <summary>
        /// 当主键由数据库生成时用此方法Add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
		Task AddAsync<T>(T item) where T : class;
        void Remove<T>(T item);
		Task RemoveAsync<T>(T item) where T : class;
		IQueryable<T> AsQueryable<T>() where T : class;
	}
	public interface IRepository<T> where T : class, IAggregateRoot
    {
        Task<T> GetByIdAsync(object id);
		Task AddAsync(T item);
		Task UpdateAsync(T item);
		Task RemoveAsync(T item);
		IQueryable<T> AsQueryable();
	}
}
