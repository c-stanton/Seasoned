namespace Seasoned.Backend.DTOs;

public class ChefConsultResponseDto
{
    public string Reply { get; set; } = string.Empty;
    public RecipeResponseDto? Recipe { get; set; }
}