using Microsoft.AspNetCore.Mvc;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;
using Seasoned.Backend.Data;
using Seasoned.Backend.Models;

namespace Seasoned.Backend.Controllers;

[ApiController]
[Route("api/recipe")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly ApplicationDbContext _context;

    public RecipeController(IRecipeService recipeService, ApplicationDbContext context)
    {
        _recipeService = recipeService;
        _context = context;
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

    [HttpPost("save")]
    public async Task<IActionResult> SaveRecipe([FromBody] RecipeResponseDto recipeDto)
    {
        if (recipeDto == null)
        {
            return BadRequest("Invalid recipe data.");
        }

        var recipe = new Recipe
        {
            Title = recipeDto.Title,
            Description = recipeDto.Description,
            Ingredients = recipeDto.Ingredients,
            Instructions = recipeDto.Instructions,
            CreatedAt = DateTime.UtcNow
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe saved!" });
    }


}