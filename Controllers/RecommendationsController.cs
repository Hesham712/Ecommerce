using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationsController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        
        [HttpGet("getRecommended")]
        public async Task<ActionResult<List<int>>> GetRecommendedProducts([FromQuery] string userId, [FromQuery] int topN = 10)
        {
           
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required.");
            }

            // Get recommendations for the user
            var recommendedProductIds = await _recommendationService.GetRecommendedProducts(userId, topN);

            if (recommendedProductIds == null || recommendedProductIds.Count == 0)
            {
                return NotFound("No recommendations found.");
            }

            return Ok(recommendedProductIds);
        }
    }
}
