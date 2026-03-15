using Microsoft.AspNetCore.Mvc;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;
using Seasoned.Backend.Data;
using System.Security.Claims;
using Seasoned.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

    [Authorize]
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
            Icon = recipeDto.Icon,
            Ingredients = recipeDto.Ingredients,
            Instructions = recipeDto.Instructions,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe saved to your collection!" });
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe updatedRecipe)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingRecipe = await _context.Recipes
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (existingRecipe == null)
        {
            return NotFound("Recipe not found or you do not have permission to edit it.");
        }

        existingRecipe.Title = updatedRecipe.Title;
        existingRecipe.Ingredients = updatedRecipe.Ingredients;
        existingRecipe.Instructions = updatedRecipe.Instructions;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe updated successfully!" });
    }

    [Authorize]
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
}