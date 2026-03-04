using Seasoned.Backend.DTOs;
using Mscc.GenerativeAI;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Seasoned.Backend.Services;

public class RecipeService : IRecipeService
{
    private readonly string _apiKey;

    public RecipeService(IConfiguration config)
    {
        _apiKey = config["GeminiApiKey"] ?? throw new ArgumentNullException("API Key missing");
    }

    public async Task<RecipeResponseDto> ParseRecipeImageAsync(IFormFile image)
    {
        var googleAI = new GoogleAI(_apiKey);
        var model = googleAI.GenerativeModel("gemini-2.5-flash"); 

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // 1. Better Prompt: Tell Gemini exactly what the JSON should look like
        var prompt = @"Extract the recipe from this image. 
        Return a JSON object with exactly these fields:
        {
        ""title"": ""string"",
        ""description"": ""string"",
        ""ingredients"": [""string""],
        ""instructions"": [""string""]
        }";
        
        // 2. Set the Response MIME Type to application/json
        var config = new GenerationConfig { ResponseMimeType = "application/json" };
        var request = new GenerateContentRequest(prompt, config);
        request.AddMedia(base64Image, "image/png");

        var response = await model.GenerateContent(request);

        // 3. Use System.Text.Json to turn that string back into our DTO
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = System.Text.Json.JsonSerializer.Deserialize<RecipeResponseDto>(response.Text, options);

        return result ?? new RecipeResponseDto { Title = "Error parsing JSON" };
    }
}