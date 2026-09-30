
using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IBrandRepository
{
    Task<IEnumerable<ProductBrand>> GetAllBrand();
    Task<ProductBrand> GetByIdAsync(string id);
}
