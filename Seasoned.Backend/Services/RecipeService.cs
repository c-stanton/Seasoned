using Seasoned.Backend.DTOs;
using Mscc.GenerativeAI;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Pgvector;

namespace Seasoned.Backend.Services;

public class RecipeService : IRecipeService
{
    private readonly string _apiKey;
    private readonly GoogleAI _googleAI;

    public RecipeService(IConfiguration config)
    {
        _apiKey = config["GEMINI_API_KEY"] ?? throw new ArgumentNullException("API Key missing");
        _googleAI = new GoogleAI(_apiKey);
    }

    public async Task<Vector> GetEmbeddingAsync(string text)
    {
        var model = _googleAI.GenerativeModel("embedding-001");
        
        var response = await model.EmbedContent(text);
    
        if (response.Embedding?.Values != null)
        {
            return new Vector(response.Embedding.Values.ToArray());
        }

    throw new Exception("The Chef couldn't extract the meaning from that recipe text.");
    }

    public async Task<RecipeResponseDto> ParseRecipeImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
                return new RecipeResponseDto { Title = "Error: No image provided" };
        }
        
        var model = _googleAI.GenerativeModel("gemini-3.1-flash-lite-preview"); 

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        var prompt = @"Extract the recipe details from this image. 
        Return ONLY a raw JSON string. 
        DO NOT include markdown formatting. 
        JSON structure:
        {
        ""title"": ""string"",
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

        string cleanJson = CleanJsonResponse(rawText);

        if (string.IsNullOrEmpty(cleanJson)) 
        {
            return new RecipeResponseDto { Title = "Error: Invalid AI Response" };
        }

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<RecipeResponseDto>(cleanJson, options);
            
            if (result != null)
            {
                result.ImageUrl = $"data:{image.ContentType};base64,{base64Image}";
            }

            return result ?? new RecipeResponseDto { Title = "Empty Response" };
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Chef's Error: JSON Parsing failed. Message: {ex.Message}");
            Console.WriteLine($"Raw AI Output: {rawText}");
            return new RecipeResponseDto { Title = "Parsing Error" };
        }
    }

    public async Task<ChefConsultResponseDto> ConsultChefAsync(string userPrompt)
    {
        var model = _googleAI.GenerativeModel("gemini-3.1-flash-lite-preview");

        var systemPrompt = @"You are the 'Seasoned' Head Chef, a master of real-world culinary arts. 
        You operate a professional kitchen and only provide advice that can be used in a real kitchen.

        STRICT CONTENT RULES:
        1. REAL FOOD ONLY: You specialize in real-world ingredients and techniques. 
        2. CELEBRITY CHEFS: You can provide recipes from real chefs like Gordon Ramsay or Julia Child.
        3. FICTIONAL FOOD TRANSLATION: If a user asks for food from a game (like a Skyrim Sweetroll) or movie, 
        do NOT give game mechanics. Instead, provide a REAL-WORLD recipe that recreates that item. 
        In your 'reply', treat it like a fun culinary challenge.
        4. REFUSAL POLICY: If the user asks about non-food topics (video game strategies, tech support, politics, or 'Minecraft crafting grids'), 
        politely refuse. Stay in character: 'The Chef's Grimoire is for spices, not spells' or 'I deal in pans, not pixels.'

        RESPONSE FORMAT:
        You MUST return ONLY a raw JSON object with these keys:
        {
            ""reply"": ""A friendly, thematic response from the Chef."",
            ""recipe"": {
                ""title"": ""string"",
                ""ingredients"": [""string"", ""string""],
                ""instructions"": [""string"", ""string""]
            } 
        }
        Note: Set the 'recipe' object to null if you are only chatting or refusing a non-food prompt.";

        var fullPrompt = $"{systemPrompt}\n\nUser Question: {userPrompt}";

        var generationConfig = new GenerationConfig { 
            ResponseMimeType = "application/json",
            Temperature = 0.7f
        };

        var request = new GenerateContentRequest(fullPrompt, generationConfig);
        var response = await model.GenerateContent(request);

        string rawText = response.Text ?? "";
        string jsonToParse = CleanJsonResponse(rawText);

        if (string.IsNullOrEmpty(jsonToParse))
            return new ChefConsultResponseDto { Reply = "The Chef is at a loss for words. Try rephrasing?" };

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ChefConsultResponseDto>(jsonToParse, options);

            return result ?? new ChefConsultResponseDto { Reply = "Chef is a bit confused!" };
        }
        catch (JsonException)
        {
            return new ChefConsultResponseDto { Reply = "The kitchen is a mess right now. Try again?" };
        }
    }

    internal string CleanJsonResponse(string rawText)
    {
        int start = rawText.IndexOf('{');
        int end = rawText.LastIndexOf('}');
        if (start == -1 || end == -1) return string.Empty;
        return rawText.Substring(start, (end - start) + 1);
    }
    
}