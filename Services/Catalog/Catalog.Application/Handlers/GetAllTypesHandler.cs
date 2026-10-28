using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers;

public record GetAllTypesHandler : IRequestHandler<GetAllTypeQuery, IList<TypeResponse>>
{
    private readonly ITypeRepository _typeRepository;
    public GetAllTypesHandler(ITypeRepository typeRepository)
    {
        _typeRepository = typeRepository;
    }

    public async Task<IList<TypeResponse>> Handle(GetAllTypeQuery request, CancellationToken cancellationToken)
    {
        var typeList = await _typeRepository.GetAllTypes();
        return typeList.ToResponseList();
    }
}
