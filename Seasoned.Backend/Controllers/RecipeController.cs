using Microsoft.AspNetCore.Mvc;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;

namespace Seasoned.Backend.Controllers;

[ApiController]
[Route("api/recipe")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<RecipeResponseDto>> UploadRecipe([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No image uploaded.");
        }

        var result = await _recipeService.ParseRecipeImageAsync(image);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetRecipes()
    {
        // This assumes your DbContext is injected as _context
        var recipes = await _context.Recipes
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
            
        return Ok(recipes);
    }
}