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

    public async Task<ChefConsultResponseDto> ConsultChefAsync(string userPrompt)
    {
        var googleAI = new GoogleAI(_apiKey);
        var model = googleAI.GenerativeModel("gemini-3.1-flash-lite-preview");

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
                ""icon"": ""string (must be a valid mdi- icon name)"",
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

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string jsonToParse = response.Text ?? "{ \"reply\": \"The chef is speechless. Try again?\" }";

            var result = JsonSerializer.Deserialize<ChefConsultResponseDto>(jsonToParse, options);

            return result ?? new ChefConsultResponseDto { Reply = "Chef is a bit confused!" };
        }
        catch (JsonException)
        {
            return new ChefConsultResponseDto { Reply = "The kitchen is a mess right now. Try again?" };
        }
    }
}