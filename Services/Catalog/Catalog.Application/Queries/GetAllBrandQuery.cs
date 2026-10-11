using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries;

public record GetAllBrandQuery : IRequest<IList<BrandResponse>>
{

}
