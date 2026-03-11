namespace Seasoned.Backend.Models;

public class Recipe {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "mdi-silverware-fork-knife";
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; } 
}