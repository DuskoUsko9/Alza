using Alza.EShop.Domain.Entities;

namespace Alza.EShop.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
}
