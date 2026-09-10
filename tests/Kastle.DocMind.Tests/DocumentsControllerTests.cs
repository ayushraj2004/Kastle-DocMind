using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;

namespace Kastle.DocMind.Tests;

public class DocumentsControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DocumentsControllerTests(
        WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetDocuments_Should_Return_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/documents");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UploadDocument_Should_Return_Created()
    {
        // Arrange
        var client = _factory.CreateClient();

        var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(
            System.Text.Encoding.UTF8.GetBytes(
                "# DocMind Test\n\nThis is an integration test document."));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("text/markdown");

        content.Add(
            fileContent,
            "file",
            "integration-test.md");

        // Act
        var response = await client.PostAsync(
            "/documents",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }
    [Fact]
    public async Task UploadDocument_Should_Return_BadRequest_For_Unsupported_File()
    {
        // Arrange
        var client = _factory.CreateClient();

        var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(
            System.Text.Encoding.UTF8.GetBytes(
                "This is a fake PDF content."));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/pdf");

        content.Add(
            fileContent,
            "file",
            "test.pdf");

        // Act
        var response = await client.PostAsync(
            "/documents",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
    [Fact]
    public async Task UploadDocument_Should_Return_BadRequest_For_Empty_File()
    {
        // Arrange
        var client = _factory.CreateClient();

        var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(Array.Empty<byte>());

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("text/plain");

        content.Add(
            fileContent,
            "file",
            "empty.txt");

        // Act
        var response = await client.PostAsync(
            "/documents",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
    [Fact]
    public async Task UploadDocument_Should_Return_BadRequest_When_File_Is_Missing()
    {
        // Arrange
        var client = _factory.CreateClient();
    
        var content = new MultipartFormDataContent();
    
        // Act
        var response = await client.PostAsync(
            "/documents",
            content);
    
        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}