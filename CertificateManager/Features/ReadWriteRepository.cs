using System.Diagnostics;
using System.Linq.Expressions;
using CertificateManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CertificateManager.Features
{
    public sealed class ReadWriteRepository : IReadWriteRepository
    {
        
        private readonly DbContext _context;    

        public ReadWriteRepository(DbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            try
            {
                var result = _context.SaveChangesAsync().Result;
            }
            catch (Exception e)
            {
                if (e.StackTrace != null) Log.Error(e.StackTrace!);
            }
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
            int? skip = null, int? take = null) 
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

        public void Create<T, TId>(T entity) where T : Entity<TId> where TId : struct
        {
            _context.Set<T>().Add(entity);
        }

        public void Update<T, TId>(T entity) where T : Entity<TId> where TId : struct
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<T, TId>(T entity) where T : Entity<TId> where TId : struct
        {
            var dbSet = _context.Set<T>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            dbSet.Remove(entity);

        }

        public void Delete<T, TId>(TId id) where T : Entity<TId> where TId : struct
        {
            var entity = _context.Set<T>().Find(id);
            
            Delete<T, TId>(entity!);
            
        }

        
        /// <summary>
        /// </summary>
        /// 
        /// <remarks>
        /// WARNING!!
        /// This should not be used in most cases. Instead disposable will take care of saving to the DB!!
        /// WARNING!!
        /// </remarks>
        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}