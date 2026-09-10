using Kastle.DocMind.Business.Services;
using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using SharpCompress.Common;
namespace Kastle.DocMind.Tests;
public class DocumentServiceTests
{
    [Fact]
    public async Task UploadAsync_Should_Save_File_And_Create_Document()
    {
        var repository=new Mock<IDocumentRepository>();
        var fileStorage=new Mock<IFileStorage>();
        var extractorResolver=new Mock<ITextExtractorResolver>();
        var extractor=new Mock<ITextExtractor>();
        var logger=new Mock<ILogger<DocumentService>>();

        extractorResolver.Setup(x=>x.Resolve("test-file.md")).Returns(extractor.Object);
        extractor.Setup(x=>x.ExtractTextAsync(It.IsAny<Stream>(), "test-file.md")).ReturnsAsync("test document content");

        var expectedPath="Storage/test-file.md";

        fileStorage.Setup(x=>x.SaveAsync(It.IsAny<Stream>(),"test-file.md")).ReturnsAsync(expectedPath);
        repository.Setup(x=>x.AddAsync(It.IsAny<Document>())).ReturnsAsync((Document document)=>document);
        var service=new DocumentService(repository.Object,fileStorage.Object,extractorResolver.Object,logger.Object);
        await using var stream=new MemoryStream();

        //act

        var result=await service.UploadAsync(stream,"test-file.md","text/markdown",100);
        //assert
        Assert.NotNull(result);
        Assert.Equal("test-file.md",result.FileName);
        Assert.Equal(100,result.Size);
        Assert.Equal("text/markdown",result.ContentType);
        Assert.Equal("Uploaded",result.Status);
        Assert.Equal(expectedPath,result.FilePath);

        fileStorage.Verify(x=>x.SaveAsync(It.IsAny<Stream>(),"test-file.md"),Times.Once);
        repository.Verify(x=>x.AddAsync(It.IsAny<Document>()),Times.Once);


    }
    [Fact]
    public async Task GetByIdAsync_Should_Return_Document_When_Exists()
    {
        // Arrange
        var repository = new Mock<IDocumentRepository>();
        var fileStorage = new Mock<IFileStorage>();
        var extractorResolver = new Mock<ITextExtractorResolver>();
        var logger = new Mock<ILogger<DocumentService>>();

        var documentId = Guid.NewGuid();

        var document = new Document
        {
            Id = documentId,
            FileName = "test.md",
            Size = 100,
            ContentType = "text/markdown",
            UploadedAt = DateTime.UtcNow,
            Status = "Uploaded",
            FilePath = "Storage/test.md"
        };

        repository
            .Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);

        var service = new DocumentService(
            repository.Object,
            fileStorage.Object,
            extractorResolver.Object,
            logger.Object);

        // Act
        var result = await service.GetByIdAsync(documentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(documentId, result.Id);
        Assert.Equal("test.md", result.FileName);

        repository.Verify(
            x => x.GetByIdAsync(documentId),
            Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Document_Does_Not_Exist()
    {
        // Arrange
        var repository = new Mock<IDocumentRepository>();
        var fileStorage = new Mock<IFileStorage>();
        var extractorResolver = new Mock<ITextExtractorResolver>();
        var logger = new Mock<ILogger<DocumentService>>();

        var documentId = Guid.NewGuid();

        repository
            .Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync((Document?)null);

        var service = new DocumentService(
            repository.Object,
            fileStorage.Object,
            extractorResolver.Object,
            logger.Object);

        // Act
        var result = await service.GetByIdAsync(documentId);

        // Assert
        Assert.Null(result);

        repository.Verify(
            x => x.GetByIdAsync(documentId),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_File_And_Document()
    {
        // Arrange
        var repository = new Mock<IDocumentRepository>();
        var fileStorage = new Mock<IFileStorage>();
        var extractorResolver = new Mock<ITextExtractorResolver>();
        var logger = new Mock<ILogger<DocumentService>>();

        var documentId = Guid.NewGuid();

        var document = new Document
        {
            Id = documentId,
            FileName = "test.md",
            Size = 100,
            ContentType = "text/markdown",
            UploadedAt = DateTime.UtcNow,
            Status = "Uploaded",
            FilePath = "Storage/test.md"
        };

        repository
            .Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);

        var service = new DocumentService(
            repository.Object,
            fileStorage.Object,
            extractorResolver.Object,
            logger.Object);

        // Act
        await service.DeleteAsync(documentId);

        // Assert
        fileStorage.Verify(
            x => x.DeleteAsync(document.FilePath),
            Times.Once);

        repository.Verify(
            x => x.DeleteAsync(document),
            Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_Should_Do_Nothing_When_Document_Does_Not_Exist()
    {
        // Arrange
        var repository = new Mock<IDocumentRepository>();
        var fileStorage = new Mock<IFileStorage>();
        var extractorResolver = new Mock<ITextExtractorResolver>();
        var logger = new Mock<ILogger<DocumentService>>();
    
        var documentId = Guid.NewGuid();
    
        repository
            .Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync((Document?)null);
    
        var service = new DocumentService(
            repository.Object,
            fileStorage.Object,
            extractorResolver.Object,
            logger.Object);
    
        // Act
        await service.DeleteAsync(documentId);
    
        // Assert
        fileStorage.Verify(
            x => x.DeleteAsync(It.IsAny<string>()),
            Times.Never);
    
        repository.Verify(
            x => x.DeleteAsync(It.IsAny<Document>()),
            Times.Never);
    }
    
}