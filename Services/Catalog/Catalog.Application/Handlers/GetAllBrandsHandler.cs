using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers;

public record GetAllBrandsHandler : IRequestHandler<GetAllBrandQuery, IList<BrandResponse>>
{
    private readonly IBrandRepository _brandRepository;
    public GetAllBrandsHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }
    public async Task<IList<BrandResponse>> Handle(GetAllBrandQuery request, CancellationToken cancellationToken)
    {
        var brandList = await _brandRepository.GetAllBrand();
        return brandList.ToReponseList(); 
    }
}
