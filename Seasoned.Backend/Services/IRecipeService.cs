using Seasoned.Backend.DTOs;

namespace Seasoned.Backend.Services;

public interface IRecipeService
{
    Task<RecipeResponseDto> ParseRecipeImageAsync(IFormFile image);
}