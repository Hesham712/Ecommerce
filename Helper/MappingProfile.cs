using AutoMapper;
using Ecommerce.DTO_s.Product;
using Ecommerce.DTO_s.ProductCategory;
using Ecommerce.DTO_S.ApplicationUser;
using Ecommerce.Models;

namespace Ecommerce.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserRegisterDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap()
                .ForMember(d => d.ImagePath, s => s.Ignore())
                .ForMember(d => d.ProductCategoryId, s => s.Condition(src => src.ProductCategoryId.HasValue));
            CreateMap<Product, ProductGetDTO>().ReverseMap()
                .ForMember(d => d.ModifiedAt, s => s.MapFrom(s => DateTime.Now));
            CreateMap<Product, ProductPostDTO>().ReverseMap()
                .ForMember(d => d.CreatedAt, s => s.MapFrom(s => DateTime.Now));

            GenericCreateMaps<ProductCategory, ProductCategoryPostDTO, ProductCategoryUpdateDTO, ProductCategoryGetDTO>();
        }
        private void GenericCreateMaps<T, PostDto, PutDto, GetDto>() where T : AbstractModel
        {
            CreateMap<PostDto, T>()
                // Add current date time whithin creating a new Row
                .ForMember(d => d.CreatedAt, s => s.MapFrom(s => DateTime.Now));
            CreateMap<PutDto, T>()
                // Add current date time whithin updating a new Row
                .ForMember(d => d.ModifiedAt, s => s.MapFrom(s => DateTime.Now));
            CreateMap<GetDto, T>().ReverseMap();
        }
    }
}
