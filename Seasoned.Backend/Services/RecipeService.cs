using Seasoned.Backend.DTOs;
using Mscc.GenerativeAI;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seasoned.Backend.Services;

public class RecipeService : IRecipeService
{
    private readonly string _apiKey;

    public RecipeService(IConfiguration config)
    {
        _apiKey = config["GEMINI_API_KEY"] ?? throw new ArgumentNullException("API Key missing");
    }

    public async Task<RecipeResponseDto> ParseRecipeImageAsync(IFormFile image)
    {
        var googleAI = new GoogleAI(_apiKey);
        
        var model = googleAI.GenerativeModel("gemini-3.1-flash-lite-preview"); 

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        var prompt = @"Extract the recipe details from this image. 
        
        RULES FOR THE 'icon' FIELD:
        1. Select a valid 'Material Design Icon' name (e.g., 'mdi-pasta', 'mdi-bread-slice', 'mdi-muffin', 'mdi-pizza').
        2. If the recipe type is ambiguous or you cannot find a specific matching icon, you MUST return 'mdi-silverware-fork-knife'.

        IMPORTANT: Return ONLY a raw JSON string. 
        DO NOT include markdown formatting. 
        JSON structure:
        {
          ""title"": ""string"",
          ""icon"": ""string"",
          ""ingredients"": [""string"", ""string""],
          ""instructions"": [""string"", ""string""]
        }";
        
        var generationConfig = new GenerationConfig { 
            ResponseMimeType = "application/json",
            Temperature = 0.1f
        };

        var request = new GenerateContentRequest(prompt, generationConfig);
    
        await Task.Run(() => request.AddMedia(base64Image, image.ContentType ?? "image/png"));

        var response = await model.GenerateContent(request);
        string rawText = response.Text?.Trim() ?? "";

        int start = rawText.IndexOf('{');
        int end = rawText.LastIndexOf('}');

        if (start == -1 || end == -1) 
        {
            return new RecipeResponseDto { Title = "Error" };
        }

        string cleanJson = rawText.Substring(start, (end - start) + 1);

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<RecipeResponseDto>(cleanJson, options);
            
            if (result != null && string.IsNullOrEmpty(result.Icon))
            {
                result.Icon = "mdi-silverware-fork-knife";
            }

            return result ?? new RecipeResponseDto { Title = "Empty Response" };
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Raw AI Output: {rawText}");
            Console.WriteLine($"Failed to parse JSON: {ex.Message}");
            
            return new RecipeResponseDto { Title = "Parsing Error" };
        }
    }
}