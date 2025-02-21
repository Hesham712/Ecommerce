using Ecommerce.DTO_s.ErrorsDTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Repository.GenericService
{
    public interface IGenericBasicDataRepo<T, TDbContext> where T : AbstractModel where TDbContext : DbContext
    {
        Task<T?> GetByIdAsync(int Id);

        Task<T> GetByIdAsync(int Id, params string[] includes);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(params string[] includes);

        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector, params string[] includes);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(int id);
        Task<ErrorsDTO> IsTypeExists(int id);

    }

}
