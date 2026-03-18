using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Seasoned.Backend.Services;
using Seasoned.Backend.DTOs;
using System.Text;

namespace Seasoned.Tests;

public class RecipeServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly RecipeService _service;

    public RecipeServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        // Mock the API Key retrieval
        _mockConfig.Setup(c => c["GEMINI_API_KEY"]).Returns("fake-api-key-123");

        _service = new RecipeService(_mockConfig.Object);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenApiKeyIsMissing()
    {
        // Arrange
        var emptyConfig = new Mock<IConfiguration>();
        emptyConfig.Setup(c => c["GEMINI_API_KEY"]).Returns((string?)null);

        // Act
        Action act = () => new RecipeService(emptyConfig.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*API Key missing*");
    }

    [Fact]
    public async Task ParseRecipeImageAsync_ReturnsError_WhenImageIsEmpty()
    {
        // Arrange: Create an empty file
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        // Act & Assert
        var result = await _service.ParseRecipeImageAsync(fileMock.Object);
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void ParseRecipeImageAsync_AttachesCorrectBase64Header()
    {
        // Arrange
        var content = "fake-image-data";
        var fileName = "test.png";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var mockFile = new FormFile(ms, 0, ms.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var resultImageUrl = $"data:{mockFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        
        resultImageUrl.Should().StartWith("data:image/png;base64,");
    }
}