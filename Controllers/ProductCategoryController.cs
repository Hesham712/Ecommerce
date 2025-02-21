using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.ProductCategory;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : GenericController<EcommerceDBContext, ProductCategory, ProductCategoryGetDTO, ProductCategoryGetDTO, ProductCategoryPostDTO, ProductCategoryUpdateDTO>
    {
        public ProductCategoryController(IGenericBasicDataRepo<ProductCategory, EcommerceDBContext> repo, IMapper mapper) : base(repo, mapper)
        {
        }
    }
}
