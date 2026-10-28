using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specifications;

namespace Catalog.Application.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(this Product product)
    {
        if(product == null) return null;
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Summary = product.Summary,
            Description = product.Description,
            ImageFile = product.ImageFile,
            Price = product.Price,
            Brand = product.Brand,
            Type = product.Type,
            CreatedDate = product.CreatedDate
        };
    }

    public static Pagination<ProductResponse> ToResponseList(this Pagination<Product> products)
    {
        return new Pagination<ProductResponse>(
            products.PageIndex,
            products.PageSize,
            products.Count,
            products.Data.Select(product => product.ToResponse()).ToList()
        );
    }
}
