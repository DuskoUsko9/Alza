using Alza.EShop.Domain.Common;
using Alza.EShop.Domain.Interfaces;
using Alza.EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alza.EShop.Infrastructure.Repositories;

public class Repository<T>(EShopDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly EShopDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}
