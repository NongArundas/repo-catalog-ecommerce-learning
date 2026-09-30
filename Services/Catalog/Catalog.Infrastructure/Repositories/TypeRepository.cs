using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class TypeRepository : ITypeRepository
{
    private readonly IMongoCollection<ProductType> type;
    public TypeRepository(IConfiguration config)
    {
        var client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
        var db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
        type = db.GetCollection<ProductType>(config["DatabaseSettings:TypeCollectionName"]);
    }
    public async Task<IEnumerable<ProductType>> GetAllTypes()
    {
        return await type.Find(_ => true).ToListAsync();
    }

    public async Task<ProductType> GetByIdAsync(string id)
    {
        return await type.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
