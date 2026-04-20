using System;
using Moq;
using FluentAssertions;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Models.Enum;
using Remp.Remp.Service;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Remp.Tests;

public class MediaAssetServiceTest
{
    private Mock<IMediaAssetRepository> _mediaAssetRepositoryMock;
    private Mock<IBlobStorageService> _blobStorageServiceMock;

    public MediaAssetServiceTest()
    {
        _mediaAssetRepositoryMock = new Mock<IMediaAssetRepository>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
    }

    // GetMediaByListingCaseIdAsync
    [Fact]
    public async Task GetMediaByListingCaseIdAsync_WhenMediaExists_ReturnGroupedMedia()
    {
        // Arrange
        var mediaAssets = new List<MediaAsset>
        {
            new MediaAsset { Id = 1, MediaType = MediaType.Photos, MediaUrl = "https://test.com/photo1.jpg", ListingCaseId = 1, IsDeleted = false },
            new MediaAsset { Id = 2, MediaType = MediaType.Photos, MediaUrl = "https://test.com/photo2.jpg", ListingCaseId = 1, IsDeleted = false },
            new MediaAsset { Id = 3, MediaType = MediaType.Videography, MediaUrl = "https://test.com/video1.mp4", ListingCaseId = 1, IsDeleted = false }
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByListingCaseIdAsync(1)).ReturnsAsync(mediaAssets);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.GetMediaByListingCaseIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2); // 2 groups: Photos and Videography
    }

    [Fact]
    public async Task GetMediaByListingCaseIdAsync_WhenNoMedia_ReturnEmptyList()
    {
        // Arrange
        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByListingCaseIdAsync(1)).ReturnsAsync(new List<MediaAsset>());

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.GetMediaByListingCaseIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    // DeleteMediaAsync
    [Fact]
    public async Task DeleteMediaAsync_WhenOwnerDeletes_ReturnTrue()
    {
        // Arrange
        var mediaAsset = new MediaAsset
        {
            Id = 1,
            UserId = "user-123",
            ListingCaseId = 1,
            IsDeleted = false
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByIdAsync(1)).ReturnsAsync(mediaAsset);
        _mediaAssetRepositoryMock.Setup(repo => repo.DeleteMediaAsync(1)).ReturnsAsync(true);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.DeleteMediaAsync(1, "user-123");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMediaAsync_WhenNotOwner_ThrowUnauthorizedAccessException()
    {
        // Arrange
        var mediaAsset = new MediaAsset
        {
            Id = 1,
            UserId = "user-123",
            ListingCaseId = 1
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByIdAsync(1)).ReturnsAsync(mediaAsset);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteMediaAsync(1, "other-user"));
    }

    [Fact]
    public async Task DeleteMediaAsync_WhenMediaNotFound_ThrowKeyNotFoundException()
    {
        // Arrange
        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByIdAsync(999)).ThrowsAsync(new KeyNotFoundException("Media asset with ID 999 not found."));

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteMediaAsync(999, "user-123"));
    }

    // UploadMediaAssetsAsync
    [Fact]
    public async Task UploadMediaAssetsAsync_WhenSinglePhotoUpload_ReturnMediaAsset()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var files = new List<IFormFile> { fileMock.Object };

        _blobStorageServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("https://test.com/photo.jpg");

        _mediaAssetRepositoryMock.Setup(repo => repo.AddMediaAssetAsync(It.IsAny<MediaAsset>())).ReturnsAsync(new MediaAsset
        {
            Id = 1,
            MediaType = MediaType.Photos,
            MediaUrl = "https://test.com/photo.jpg",
            UploadedAt = DateTime.UtcNow,
            IsSelect = false,
            IsHero = false,
            ListingCaseId = 1,
            UserId = "user-123",
            IsDeleted = false
        });

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.UploadMediaAssetsAsync(files, MediaType.Photos, 1, "user-123");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].MediaUrl.Should().Be("https://test.com/photo.jpg");
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenMultipleVideoUpload_ThrowInvalidOperationException()
    {
        // Arrange
        var fileMock1 = new Mock<IFormFile>();
        var fileMock2 = new Mock<IFormFile>();
        var files = new List<IFormFile> { fileMock1.Object, fileMock2.Object };

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadMediaAssetsAsync(files, MediaType.Videography, 1, "user-123"));
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenNoFiles_ThrowInvalidOperationException()
    {
        // Arrange
        var files = new List<IFormFile>();

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadMediaAssetsAsync(files, MediaType.Photos, 1, "user-123"));
    }

    // SelectMediaAsync
    [Fact]
    public async Task SelectMediaAsync_WhenValidSelection_ReturnSelectedMedia()
    {
        // Arrange
        var allMedia = new List<MediaAsset>
        {
            new MediaAsset { Id = 1, MediaType = MediaType.Photos, ListingCaseId = 1, IsSelect = false, IsDeleted = false },
            new MediaAsset { Id = 2, MediaType = MediaType.Photos, ListingCaseId = 1, IsSelect = false, IsDeleted = false }
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByListingCaseIdAsync(1)).ReturnsAsync(allMedia);
        _mediaAssetRepositoryMock.Setup(repo => repo.UpdateMediaAssetAsync(It.IsAny<MediaAsset>())).ReturnsAsync((MediaAsset m) => m);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.SelectMediaAsync(1, new List<int> { 1, 2 });

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.All(m => m.IsSelect).Should().BeTrue();
    }

    [Fact]
    public async Task SelectMediaAsync_WhenMediaNotBelongToListing_ThrowInvalidOperationException()
    {
        // Arrange
        var allMedia = new List<MediaAsset>
        {
            new MediaAsset { Id = 1, MediaType = MediaType.Photos, ListingCaseId = 1, IsDeleted = false }
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByListingCaseIdAsync(1)).ReturnsAsync(allMedia);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SelectMediaAsync(1, new List<int> { 1, 999 }));
    }

    [Fact]
    public async Task SelectMediaAsync_WhenMoreThan10Selected_ThrowInvalidOperationException()
    {
        // Arrange
        var mediaIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SelectMediaAsync(1, mediaIds));
    }

    // SetCoverImageAsync
    [Fact]
    public async Task SetCoverImageAsync_WhenValidMedia_ReturnUpdatedMedia()
    {
        // Arrange
        var mediaAsset = new MediaAsset
        {
            Id = 1,
            MediaType = MediaType.Photos,
            ListingCaseId = 1,
            IsHero = false,
            IsDeleted = false
        };

        var allMedia = new List<MediaAsset> { mediaAsset };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByIdAsync(1)).ReturnsAsync(mediaAsset);
        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByListingCaseIdAsync(1)).ReturnsAsync(allMedia);
        _mediaAssetRepositoryMock.Setup(repo => repo.UpdateMediaAssetAsync(It.IsAny<MediaAsset>())).ReturnsAsync((MediaAsset m) => m);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act
        var result = await service.SetCoverImageAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result.IsHero.Should().BeTrue();
    }

    [Fact]
    public async Task SetCoverImageAsync_WhenMediaNotBelongToListing_ThrowInvalidOperationException()
    {
        // Arrange
        var mediaAsset = new MediaAsset
        {
            Id = 1,
            ListingCaseId = 2, // different listing
            IsDeleted = false
        };

        _mediaAssetRepositoryMock.Setup(repo => repo.GetMediaByIdAsync(1)).ReturnsAsync(mediaAsset);

        var service = new MediaAssetService(_mediaAssetRepositoryMock.Object, _blobStorageServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SetCoverImageAsync(1, 1));
    }
}