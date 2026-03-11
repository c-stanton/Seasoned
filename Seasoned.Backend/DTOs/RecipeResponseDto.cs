namespace Seasoned.Backend.DTOs;

public class RecipeResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "mdi-silverware-fork-knife";
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
}