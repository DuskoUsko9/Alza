using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Domain.Entities;
using AutoMapper;

namespace Alza.EShop.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between DTOs and domain entities.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Request to Entity
        CreateMap<CreateProductRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Entity to Response
        CreateMap<Product, ProductResponse>();
    }
}
