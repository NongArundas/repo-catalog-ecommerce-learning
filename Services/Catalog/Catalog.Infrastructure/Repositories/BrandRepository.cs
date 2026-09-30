
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IMongoCollection<ProductBrand> brands;
    public BrandRepository(IConfiguration config)
    {
        var client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
        var db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
        brands = db.GetCollection<ProductBrand>(config["DatabaseSettings:BrandCollectionName"]);
    }

    public async Task<IEnumerable<ProductBrand>> GetAllBrand()
    {
        return await brands.Find(_ => true).ToListAsync();
    }

    public async Task<ProductBrand> GetByIdAsync(string id)
    {
        return await brands.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

}
