using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Play.Inventory.Service.Entities;


namespace Play.Catalog.Service.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(int id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(int id);
        //  Task RemoveAsync(Expression<Func<T, bool>> filter);

    }
}