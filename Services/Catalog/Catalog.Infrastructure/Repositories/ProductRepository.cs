using System;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductBrand> brands;
    private readonly IMongoCollection<ProductType> types;
    private readonly IMongoCollection<Product> products;
    public ProductRepository(IOptions<DatabaseSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
        types = db.GetCollection<ProductType>(settings.TypeCollectionName);
        products = db.GetCollection<Product>(settings.ProductCollectionName);

    }
    public async Task<Product> CreateProduct(Product product)
    {
        await products.InsertOneAsync(product);
        return product;
    }

    public async Task<bool> DeleteProduct(string productId)
    {
        var deletedProduct = await products.DeleteOneAsync(x => x.Id == productId);
        return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await products.Find(_ => true).ToListAsync();
    }

    public async Task<ProductBrand> GetBrandsByIdAsync(string brandId)
    {
        return await brands.Find(x => x.Id == brandId).FirstOrDefaultAsync();
    }

    public async Task<Product> GetProduct(string id)
    {
        return await products.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
    {
        var builder = Builders<Product>.Filter;
        var filter = builder.Empty;
        if(!string.IsNullOrEmpty(catalogSpecParams.Search))
        {
            filter &= builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
        }
        if(!string.IsNullOrEmpty(catalogSpecParams.BrandId))
        {
            filter &= builder.Eq(p => p.Brand.Id, catalogSpecParams.BrandId);
        }
        if(!string.IsNullOrEmpty(catalogSpecParams.TypeId))
        {
            filter &= builder.Eq(p => p.Type.Id, catalogSpecParams.TypeId);
        }

        var totalItems = await products.CountDocumentsAsync(filter);
        var data = await ApplyDataFilter(catalogSpecParams, filter);
        return new Pagination<Product>(
            catalogSpecParams.PageIndex,
            catalogSpecParams.PageSize,
            (int)totalItems,
            data
        );
    }

    public async Task<IEnumerable<Product>> GetProductsByBrand(string name)
    {
        return await products.Find(x => x.Brand.Name.ToLower() == name.ToLower()).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByName(string name)
    {
        var filter = Builders<Product>.Filter.Regex(x => x.Name, new BsonRegularExpression($".*{name}.*", "i"));
        return await products.Find(filter).ToListAsync();
    }

    public async Task<ProductType> GetTypesByIdAsync(string typeId)
    {
        return await types.Find(x => x.Id == typeId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updatedProduct = await products.ReplaceOneAsync(x => x.Id == product.Id, product);
        return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
    }

    private async Task<IReadOnlyCollection<Product>> ApplyDataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
    {
        var sortDefn = Builders<Product>.Sort.Ascending("Name");
        if(!string.IsNullOrEmpty(catalogSpecParams.Sort))
        {
            sortDefn = catalogSpecParams.Sort switch
            {
                "priceAsc" => Builders<Product>.Sort.Ascending(p => p.Price),
                "priceDesc" => Builders<Product>.Sort.Descending(p => p.Price),
                _ => Builders<Product>.Sort.Ascending(p => p.Name)
            };
        }
        return await products
        .Find(filter)
        .Sort(sortDefn)
        .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
        .Limit(catalogSpecParams.PageSize)
        .ToListAsync();
    }
}
