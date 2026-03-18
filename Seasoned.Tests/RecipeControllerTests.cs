using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seasoned.Backend.Controllers;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;
using Seasoned.Backend.Data;
using Seasoned.Backend.Models;
using System.Security.Claims;

namespace Seasoned.Tests;

public class RecipeControllerTests
{
    private readonly Mock<IRecipeService> _mockService;
    private readonly ApplicationDbContext _context;
    private readonly RecipeController _controller;
    private readonly string _testUserId = "chef-123";

    public RecipeControllerTests()
    {
        _mockService = new Mock<IRecipeService>();
        
        // Setup InMemory Postgres replacement
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _controller = new RecipeController(_mockService.Object, _context);

        // Mock the User Identity (User.FindFirstValue(ClaimTypes.NameIdentifier))
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task UploadRecipe_ReturnsOk_WithParsedDataAndImage()
    {
        // Arrange
        var fakeRecipe = new RecipeResponseDto 
        { 
            Title = "Roasted Garlic Pasta",
            ImageUrl = "data:image/png;base64,header_captured_image" 
        };
        
        _mockService.Setup(s => s.ParseRecipeImageAsync(It.IsAny<IFormFile>()))
                    .ReturnsAsync(fakeRecipe);

        var file = CreateMockFile("test-recipe.jpg");

        // Act
        var result = await _controller.UploadRecipe(file);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returned = okResult!.Value as RecipeResponseDto;
        returned!.Title.Should().Be("Roasted Garlic Pasta");
        returned.ImageUrl.Should().Contain("base64");
    }

    [Fact]
    public async Task SaveRecipe_PersistsToPostgres_ForCurrentUser()
    {
        // Arrange
        var dto = new RecipeResponseDto
        {
            Title = "Jenkins Special Brew",
            Ingredients = new List<string> { "Coffee", "Code" },
            ImageUrl = "base64-string"
        };

        // Act
        var result = await _controller.SaveRecipe(dto);

        // Assert
        var okResult = result as OkResult; // Or OkObjectResult based on your Controller return
        var savedRecipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Title == "Jenkins Special Brew");
        
        savedRecipe.Should().NotBeNull();
        savedRecipe!.UserId.Should().Be(_testUserId);
        savedRecipe.ImageUrl.Should().Be("base64-string");
    }

    [Fact]
    public async Task ConsultChef_ReturnsChefResponse()
    {
        // Arrange
        var request = new ChatRequestDto { Prompt = "How do I sear a steak?" };
        var expectedResponse = new ChefConsultResponseDto { Reply = "High heat, my friend!" };
        
        _mockService.Setup(s => s.ConsultChefAsync(It.IsAny<string>()))
                    .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Consult(request);

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as ChefConsultResponseDto;
        response!.Reply.Should().Be("High heat, my friend!");
    }

    private IFormFile CreateMockFile(string fileName)
    {
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("fake image binary data");
        writer.Flush();
        ms.Position = 0;
        return new FormFile(ms, 0, ms.Length, "image", fileName);
    }
}