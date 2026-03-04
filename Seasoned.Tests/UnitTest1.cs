using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Seasoned.Backend.Controllers;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;

namespace Seasoned.Tests;

public class RecipeControllerTests
{
    [Fact]
    public async Task ParseRecipe_ReturnsOk_WhenImageIsValid()
    {
        // 1. Arrange: Create a "Fake" Service
        var mockService = new Mock<IRecipeService>();
        var fakeRecipe = new RecipeResponseDto { Title = "Test Recipe" };
        
        mockService.Setup(s => s.ParseImageAsync(It.IsAny<IFormFile>()))
                   .ReturnsAsync(fakeRecipe);

        var controller = new RecipeController(mockService.Object);

        // Create a fake image file
        var content = "fake image content";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        var mockFile = new FormFile(ms, 0, ms.Length, "id_from_form", fileName);

        // 2. Act: Call the Controller
        var result = await controller.ParseRecipe(mockFile);

        // 3. Assert: Check the result
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var returnedRecipe = okResult.Value as RecipeResponseDto;
        returnedRecipe!.Title.Should().Be("Test Recipe");
    }
}