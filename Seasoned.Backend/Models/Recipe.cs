using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seasoned.Backend.Models;

public class Recipe {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; } 

    [Column(TypeName = "vector(768)")]
    public Vector? Embedding { get; set; }
}