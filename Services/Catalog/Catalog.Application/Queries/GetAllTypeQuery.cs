using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries;

public record GetAllTypeQuery : IRequest<IList<TypeResponse>>
{

}
