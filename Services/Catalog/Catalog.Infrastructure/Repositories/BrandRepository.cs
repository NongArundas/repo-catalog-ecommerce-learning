
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IMongoCollection<ProductBrand> brands;
    public BrandRepository(IOptions<DatabaseSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
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
