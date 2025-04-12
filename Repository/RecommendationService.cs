using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Ecommerce.Data;
using Ecommerce.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Repository.GenericService;

namespace Ecommerce.Repository
{
    public class RecommendationService
    {
        private static MLContext _mlContext = new MLContext();
        private static ITransformer _model;

        private readonly IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> _repo;
        private readonly IGenericBasicDataRepo<Product, EcommerceDBContext> _productRepo;

        public RecommendationService(
            IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> repo,
            IGenericBasicDataRepo<Product, EcommerceDBContext> productRepo)
        {
            _repo = repo;
            _productRepo = productRepo;
            TrainModel();
        }

        // Method to train the recommendation model using UserInteraction data
        public void TrainModel()
        {
            var interactions = _repo.GetAllAsync().Result;

            if (!interactions.Any()) return;

            // Create a list of interactions with weighted scores (based on interaction type)
            var weightedInteractions = interactions.Select(i => new InteractionData
            {
                UserId = i.UserId,
                ProductId = i.ProductId,
                InteractionWeight = i.InteractionTypeId == 1 ? 1 :  // View
                                    i.InteractionTypeId == 2 ? 2 :  // Wishlist
                                    i.InteractionTypeId == 3 ? 3 : 0 // Purchase
            }).ToList();

            // Load the interaction data into ML.NET's IDataView format
            var data = _mlContext.Data.LoadFromEnumerable(weightedInteractions);

            // Define the pipeline to transform UserId and ProductId to keys and then apply matrix factorization
            var pipeline = _mlContext.Transforms.Conversion
                    .MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: nameof(InteractionData.UserId))
                .Append(_mlContext.Transforms.Conversion
                    .MapValueToKey(outputColumnName: "productIdEncoded", inputColumnName: nameof(InteractionData.ProductId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    new MatrixFactorizationTrainer.Options
                    {
                        MatrixColumnIndexColumnName = "userIdEncoded",  // Encoded UserId
                        MatrixRowIndexColumnName = "productIdEncoded",  // Encoded ProductId
                        LabelColumnName = nameof(InteractionData.InteractionWeight), // The weight of the interaction
                        NumberOfIterations = 20,
                        ApproximationRank = 100
                    }));

            // Train the model using the pipeline
            _model = pipeline.Fit(data);
        }

        // Method to get recommended products for a given user
        public async Task<List<int>> GetRecommendedProducts(string userId, int topN = 5)
        {
            // If the model is null, return an empty list of recommendations
            if (_model == null)
                return new List<int>();

            // Fetch all products from the product repository
            var allProducts = await _productRepo.GetAllAsync();

            // Create a prediction engine to make predictions based on the trained model
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<PredictionInput, RecommendationPrediction>(_model);

            // Generate predictions for all products for the given user
            var recommendations = allProducts.Select(p =>
            {
                var input = new PredictionInput
                {
                    UserId = userId,
                    Product = p
                };
                var score = predictionEngine.Predict(input).Score; // Predict the interaction score
                return new { ProductId = p.Id, Score = score };
            });

            // Order the products by the predicted score in descending order and return the topN products
            return recommendations
                .OrderByDescending(r => r.Score)
                .Take(topN)
                .Select(r => r.ProductId)
                .ToList();
        }

        // Class to represent the interaction data for training
        private class InteractionData
        {
            public string UserId { get; set; }
            public int ProductId { get; set; }
            public float InteractionWeight { get; set; }
        }

        // Class to represent the input for predictions
        private class PredictionInput
        {
            public string UserId { get; set; }
            public Product Product { get; set; }
        }

        // Class to represent the prediction output (score for recommendation)
        private class RecommendationPrediction
        {
            public float Score { get; set; }
        }
    }
}
