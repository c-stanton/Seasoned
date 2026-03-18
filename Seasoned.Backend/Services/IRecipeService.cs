using Seasoned.Backend.DTOs;

namespace Seasoned.Backend.Services;

public interface IRecipeService
{
    Task<RecipeResponseDto> ParseRecipeImageAsync(IFormFile image);
    Task<ChefConsultResponseDto> ConsultChefAsync(string userPrompt);
    Task<Pgvector.Vector> GetEmbeddingAsync(string text);
}