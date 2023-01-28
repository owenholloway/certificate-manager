using System.Linq.Expressions;
using CertificateManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features
{
    public sealed class ReadOnlyRepository : IReadOnlyRepository
    {
        private readonly DbContext _context;

        public ReadOnlyRepository(DbContext context)
        {
            _context = context;
        }
        
        public void Dispose()
        {
            //We don't need to do anything on a read repo dispose as it cannot commit data
        }

        private IQueryable<T> GetQueryable<T, TId>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            int? skip = null,
            int? take = null)
            where T : Entity<TId> where TId : struct
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        public IQueryable<T> Table<T, TId>(
            Func<IQueryable<T>, 
            IOrderedQueryable<T>>? orderBy = null, 
            string? includeProperties = null, 
            int? skip = null, 
            int? take = null) 
            where T : Entity<TId> where TId : struct
        {
            return GetQueryable<T, TId>(null, orderBy, includeProperties, skip, take);
        }

        public T GetById<T, TId>(TId id) where T : Entity<TId> where TId : struct
        {
            return _context.Set<T>().Find(id)!;
        }

        public Task<T> GetByIdAsync<T, TId>(TId id) where T : Entity<TId> where TId : struct
        {
            return _context.Set<T>().FindAsync(id).AsTask()!;
        }
    }
}