using Alza.EShop.Domain.Common;

namespace Alza.EShop.Domain.Interfaces;

/// <summary>
/// Base repository interface for generic entity operations, T must inherit from BaseEntity
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
