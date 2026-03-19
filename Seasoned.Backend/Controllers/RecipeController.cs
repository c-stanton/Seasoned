using Microsoft.AspNetCore.Mvc;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;
using Seasoned.Backend.Data;
using System.Security.Claims;
using Seasoned.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Seasoned.Backend.Controllers;

[Authorize]
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var recipe = new Recipe
        {
            Title = recipeDto.Title,
            ImageUrl = recipeDto.ImageUrl,
            Ingredients = recipeDto.Ingredients,
            Instructions = recipeDto.Instructions,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        var fullText = $"{recipeDto.Title} {string.Join(" ", recipeDto.Ingredients)} {string.Join(" ", recipeDto.Instructions)}";
        recipe.Embedding = await _recipeService.GetEmbeddingAsync(fullText);

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe saved to your collection!" });
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] RecipeUpdateDto updatedRecipe)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingRecipe = await _context.Recipes
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (existingRecipe == null)
        {
            return NotFound("Recipe not found or permission denied.");
        }

        existingRecipe.Title = updatedRecipe.Title;
        existingRecipe.Ingredients = updatedRecipe.Ingredients;
        existingRecipe.Instructions = updatedRecipe.Instructions;
        
        if (!string.IsNullOrEmpty(updatedRecipe.ImageUrl))
        {
            existingRecipe.ImageUrl = updatedRecipe.ImageUrl;
        }

        var fullText = $"{updatedRecipe.Title} {string.Join(" ", updatedRecipe.Ingredients)} {string.Join(" ", updatedRecipe.Instructions)}";
        existingRecipe.Embedding = await _recipeService.GetEmbeddingAsync(fullText);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe updated successfully!" });
    }

    [HttpGet("my-collection")]
   public async Task<ActionResult<IEnumerable<Recipe>>> GetMyRecipes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var myRecipes = await _context.Recipes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(myRecipes);
    }

    [HttpPost("consult")]
    public async Task<IActionResult> Consult([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("The Chef needs a prompt.");

        var result = await _recipeService.ConsultChefAsync(request.Prompt);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Recipe>>> SearchRecipes([FromQuery] string query)
    {
        Console.WriteLine($"--> Search hit! Query: {query}");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(query))
            return await GetMyRecipes();
            
        var queryVector = await _recipeService.GetEmbeddingAsync(query);

        var results = await _context.Recipes
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Embedding!.CosineDistance(queryVector))
            .Take(15)
            .ToListAsync();

        return Ok(results);
    }
}