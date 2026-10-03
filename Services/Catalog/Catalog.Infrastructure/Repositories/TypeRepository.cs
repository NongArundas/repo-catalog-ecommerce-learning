using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class TypeRepository : ITypeRepository
{
    private readonly IMongoCollection<ProductType> type;
    public TypeRepository(IOptions<DatabaseSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        type = db.GetCollection<ProductType>(settings.TypeCollectionName);
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
