using AutoMapper;
using Ecommerce.DTO_s.ApplicationUser;
using Ecommerce.DTO_s.Cart;
using Ecommerce.DTO_s.InteractionType;
using Ecommerce.DTO_s.Notification;
using Ecommerce.DTO_s.Order;
using Ecommerce.DTO_s.OrderProduct;
using Ecommerce.DTO_s.Product;
using Ecommerce.DTO_s.ProductCategory;
using Ecommerce.DTO_s.Rate;
using Ecommerce.DTO_s.Refund;
using Ecommerce.DTO_s.UserInteraction;
using Ecommerce.DTO_s.WishList;
using Ecommerce.DTO_S.ApplicationUser;
using Ecommerce.Models;

namespace Ecommerce.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserRegisterDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserGetDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserUpdateDTO>().ReverseMap();

            CreateMap<CartProductGetDTO, CartProduct>().ReverseMap();
            CreateMap<WishListProductGetDTO, WishListProduct>().ReverseMap();

            CreateMap<Product, ProductUpdateDTO>().ReverseMap()
                .ForMember(d => d.ImagePath, s => s.Ignore())
                .ForMember(d => d.ProductCategoryId, s => s.Condition(src => src.ProductCategoryId.HasValue));
            CreateMap<Product, ProductGetDTO>().ReverseMap()
                .ForMember(d => d.ModifiedAt, s => s.MapFrom(s => DateTime.Now))
                .ForMember(d => d.ProductCategory, s => s.MapFrom(s => s.ProductCategory));
            CreateMap<Product, ProductPostDTO>().ReverseMap()
                .ForMember(d => d.CreatedAt, s => s.MapFrom(s => DateTime.Now));
            CreateMap<Order, OrderGetDTO>()
                .ForMember(dest => dest.OrderProduct, opt => opt.MapFrom(src => src.OrderProduct));
            CreateMap<OrderProduct, OrderProductGetDTO>()
                .ForMember(d => d.ProductId, s => s.MapFrom(s => s.ProductId))
                .ForMember(d => d.OrderId, s => s.MapFrom(s => s.Order.Id))
                .ForMember(d => d.ProductName, s => s.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Price, s => s.MapFrom(s => s.Product.Price))
                .ForMember(d => d.Quantity, s => s.MapFrom(s => s.Quantity));

            CreateMap<Refund, RefundPostDTO>().ReverseMap()
            .ForMember(dest => dest.RefundItems, opt => opt.MapFrom(src => new List<RefundItem> { new RefundItem { OrderProductId = src.RefundItems.OrderProductId, Quantity = src.RefundItems.Quantity, Reason = src.RefundItems.Reason } }));
            CreateMap<RefundItem, RefundItemPostDTO>().ReverseMap();
            CreateMap<RefundItem, RefundItemGetDTO>().ReverseMap();
            CreateMap<Refund, RefundGetDTO>().ReverseMap();

            CreateMap<NotificationGetDTO, Notification>().ReverseMap();

            GenericCreateMaps<ProductCategory, ProductCategoryPostDTO, ProductCategoryUpdateDTO, ProductCategoryGetDTO>();
            GenericCreateMaps<Rate, RatePostDTO, RateUpdateDTO, RateGetDTO>();
            GenericCreateMaps<InteractionType, InteractionTypePostDTO, InteractionTypeUpdateDTO, InteractionTypeGetDTO>();
            GenericCreateMaps<UserInteraction, UserInteractionPostDTO, UserInteractionUpdateDTO, UserInteractionGetDTO>();

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
