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
        var model = googleAI.GenerativeModel("gemini-2.5-flash"); 

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        var prompt = @"Extract the recipe details from this image. 
        IMPORTANT: Return ONLY a raw JSON string. 
        DO NOT include markdown formatting (no ```json). 
        DO NOT include any text before or after the JSON.
        All property names and string values MUST be enclosed in double quotes.
        JSON structure:
        {
        ""title"": ""string"",
        ""description"": ""string"",
        ""ingredients"": [""string"", ""string""],
        ""instructions"": [""string"", ""string""]
        }";
        
        var config = new GenerationConfig { 
            ResponseMimeType = "application/json",
            Temperature = 0.1f
        };

        var request = new GenerateContentRequest(prompt, config);
        await Task.Run(() => request.AddMedia(base64Image, "image/png"));

        var response = await model.GenerateContent(request);
        string rawText = response.Text?.Trim() ?? "";

        int start = rawText.IndexOf('{');
        int end = rawText.LastIndexOf('}');

        if (start == -1 || end == -1) 
        {
            return new RecipeResponseDto { Title = "Error", Description = "AI failed to generate a valid JSON block." };
        }

        string cleanJson = rawText.Substring(start, (end - start) + 1);

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<RecipeResponseDto>(cleanJson, options);
            return result ?? new RecipeResponseDto { Title = "Empty Response" };
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Raw AI Output: {rawText}");
            Console.WriteLine($"Failed to parse JSON: {ex.Message}");
            
            return new RecipeResponseDto { 
                Title = "Parsing Error", 
                Description = "The AI response was malformed. Check logs." 
            };
        }
    }
}