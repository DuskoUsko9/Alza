using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Domain.Entities;
using AutoMapper;

namespace Alza.EShop.API.MappingProfiles;

/// <summary>
/// AutoMapper profile for Product entity mappings.
/// </summary>
public class ProductMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the ProductMappingProfile class.
    /// </summary>
    public ProductMappingProfile()
    {
        // Product -> ProductResponse
        CreateMap<Product, ProductResponse>();

        // CreateProductRequest -> Product
        CreateMap<CreateProductRequest, Product>();

        // UpdateStockRequest -> Product
        CreateMap<UpdateStockRequest, Product>();
    }
}
