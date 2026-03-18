namespace Seasoned.Backend.DTOs;

public class RecipeResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    
}