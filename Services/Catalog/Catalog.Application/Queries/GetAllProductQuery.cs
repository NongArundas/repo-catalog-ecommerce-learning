using Catalog.Application.Responses;
using Catalog.Core.Specifications;
using MediatR;

namespace Catalog.Application.Queries;

public record GetAllProductQuery(CatalogSpecParams CatalogSpecParams) : IRequest<Pagination<ProductResponse>>
{

}
