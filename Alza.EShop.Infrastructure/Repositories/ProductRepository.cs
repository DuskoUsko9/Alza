using Alza.EShop.Domain.Entities;
using Alza.EShop.Domain.Interfaces;
using Alza.EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alza.EShop.Infrastructure.Repositories;

public class ProductRepository(EShopDbContext context) : Repository<Product>(context), IProductRepository
{
    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .OrderBy(p => p.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
