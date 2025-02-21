using Ecommerce.DTO_s.ErrorsDTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Repository.GenericService
{
    public class GenericBasicDataRepo<T, TDbContext> : IGenericBasicDataRepo<T, TDbContext> where T : AbstractModel where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public GenericBasicDataRepo(TDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            T entity = await _context.Set<T>().FirstAsync((t) => t.Id == id);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _context.Set<T>().Where(predicate).Select(selector).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
        {
            IQueryable<T> source = _context.Set<T>();
            foreach (string navigationPropertyPath in includes)
            {
                source = source.Include(navigationPropertyPath);
            }

            return await source.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector, params string[] includes)
        {
            IQueryable<T> source = _context.Set<T>();
            foreach (string navigationPropertyPath in includes)
            {
                source = source.Include(navigationPropertyPath);
            }

            return await source.Select(selector).ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int Id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync((t) => t.Id == Id);
        }

        public virtual async Task<T> GetByIdAsync(int Id, params string[] includes)
        {
            IQueryable<T> source = _context.Set<T>();
            foreach (string navigationPropertyPath in includes)
            {
                source = source.Include(navigationPropertyPath);
            }

            return await source.FirstAsync((t) => t.Id == Id);
        }
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<ErrorsDTO> IsTypeExists(int id)
        {
            ErrorsDTO errors = new ErrorsDTO
            {
                IsValid = true,
                Errors = new List<ErrorDTO>()
            };
            if (!await _context.Set<T>().AnyAsync((t) => t.Id == id))
            {
                errors.IsValid = false;
                errors.Errors.Add(new ErrorDTO
                {
                    ErrorEn = "object doesn't exists",
                    ErrorAr = "هذا العنصر غير موجود"
                });
            }

            return errors;
        }
    }

}
