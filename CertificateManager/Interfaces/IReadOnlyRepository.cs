using CertificateManager.Features;

namespace CertificateManager.Interfaces
{
    public interface IReadOnlyRepository : IDisposable
    {
        IQueryable<T> Table<T, TId>(
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            int? skip = null,
            int? take = null)
            where T : Entity<TId> where TId : struct;
        
        T GetById<T, TId>(TId id) where T : Entity<TId> where TId : struct;
        
        Task<T> GetByIdAsync<T, TId>(TId id) where T : Entity<TId> where TId : struct;
    }
}