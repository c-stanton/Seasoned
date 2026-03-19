public class RecipeUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public string? ImageUrl { get; set; }
}