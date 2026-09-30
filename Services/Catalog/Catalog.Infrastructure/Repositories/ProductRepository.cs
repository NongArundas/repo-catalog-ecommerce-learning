using System;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductBrand> brands;
    private readonly IMongoCollection<ProductType> types;
    private readonly IMongoCollection<Product> products;
    public ProductRepository(IConfiguration config)
    {
        var client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
        var db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
        brands = db.GetCollection<ProductBrand>(config["DatabaseSettings:BrandCollectionName"]);
        types = db.GetCollection<ProductType>(config["DatabaseSettings:TypeCollectionName"]);
        products = db.GetCollection<Product>(config["DatabaseSettings:ProductCollectionName"]);

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

    public Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams)
    {
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
}
