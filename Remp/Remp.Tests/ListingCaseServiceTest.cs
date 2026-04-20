using System;
using Moq;
using FluentAssertions;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Enum;
using Remp.Remp.Service;
using Xunit;

namespace Remp.Tests;

public class ListingCaseServiceTest
{
    private Mock<IListingCaseRepository> _listingCaseRepositoryMock;

    public ListingCaseServiceTest()
    {
        _listingCaseRepositoryMock = new Mock<IListingCaseRepository>();
    }

    // CreateListingCaseAsync
    [Fact]
    public async Task CreateListingCaseAsync_WhenRepositorySucceeds_ReturnNewListingCase()
    {
        // Arrange
        var listingCase = new ListingCase()
        {
            Id = 1,
            Title = "Test Property",
            Description = "Test Description",
            Street = "123 Test St",
            City = "Sydney",
            State = "NSW",
            PostCode = 2000,
            Bedrooms = 3,
            Bathrooms = 2,
            Garages = 1,
            Price = 800000,
            PropertyType = PropertyType.House,
            SaleCategory = SaleCategory.ForSale,
            UserId = "user-123"
        };

        _listingCaseRepositoryMock.Setup(repo => repo.AddListingCaseAsync(It.IsAny<ListingCase>())).ReturnsAsync(listingCase);

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act
        var result = await listingCaseService.CreateListingCaseAsync(listingCase);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Property");
        result.ListcaseStatus.Should().Be(ListcaseStatus.Created);
        result.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateListingCaseAsync_WhenRepositoryFails_ThrowException()
    {
        // Arrange
        var listingCase = new ListingCase()
        {
            Id = 1,
            Title = "Test Property",
            UserId = "user-123"
        };

        _listingCaseRepositoryMock.Setup(repo => repo.AddListingCaseAsync(It.IsAny<ListingCase>())).ThrowsAsync(new Exception("Failed to add listing case."));

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => listingCaseService.CreateListingCaseAsync(listingCase));
    }

    // GetAllListingCasesAsync
    [Fact]
    public async Task GetAllListingCasesAsync_WhenRepositorySucceeds_ReturnPaginatedResult()
    {
        // Arrange
        var listingCases = new List<ListingCase>
        {
            new ListingCase { Id = 1, Title = "Property 1", IsDeleted = false },
            new ListingCase { Id = 2, Title = "Property 2", IsDeleted = false }
        };

        _listingCaseRepositoryMock.Setup(repo => repo.GetAllListingCasesAsync(1, 10, null)).ReturnsAsync((listingCases, 2));

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        var filter = new Remp.Models.DTOs.ListingCaseFilterRequestDto
        {
            PageNumber = 1,
            PageSize = 10,
            Status = null
        };

        // Act
        var result = await listingCaseService.GetAllListingCasesAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Count.Should().Be(2);
    }

    // GetListingCaseByIdAsync
    [Fact]
    public async Task GetListingCaseByIdAsync_WhenExists_ReturnListingCase()
    {
        // Arrange
        var listingCase = new ListingCase()
        {
            Id = 1,
            Title = "Test Property",
            IsDeleted = false
        };

        _listingCaseRepositoryMock.Setup(repo => repo.GetListingCaseByIdAsync(1)).ReturnsAsync(listingCase);

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act
        var result = await listingCaseService.GetListingCaseByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Property");
    }

    [Fact]
    public async Task GetListingCaseByIdAsync_WhenNotExists_ThrowKeyNotFoundException()
    {
        // Arrange
        _listingCaseRepositoryMock.Setup(repo => repo.GetListingCaseByIdAsync(999)).ThrowsAsync(new KeyNotFoundException("Listing case with ID 999 not found."));

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => listingCaseService.GetListingCaseByIdAsync(999));
    }

    // UpdateListingCaseAsync
    [Fact]
    public async Task UpdateListingCaseAsync_WhenRepositorySucceeds_ReturnUpdatedListingCase()
    {
        // Arrange
        var listingCase = new ListingCase()
        {
            Id = 1,
            Title = "Updated Property",
            Price = 900000
        };

        _listingCaseRepositoryMock.Setup(repo => repo.UpdateListingCaseAsync(It.IsAny<ListingCase>())).ReturnsAsync(listingCase);

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act
        var result = await listingCaseService.UpdateListingCaseAsync(listingCase);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Property");
        result.Price.Should().Be(900000);
    }

    // DeleteListingCaseAsync
    [Fact]
    public async Task DeleteListingCaseAsync_WhenRepositorySucceeds_ReturnTrue()
    {
        // Arrange
        _listingCaseRepositoryMock.Setup(repo => repo.DeleteListingCaseAsync(1)).ReturnsAsync(true);

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act
        var result = await listingCaseService.DeleteListingCaseAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteListingCaseAsync_WhenNotExists_ThrowException()
    {
        // Arrange
        _listingCaseRepositoryMock.Setup(repo => repo.DeleteListingCaseAsync(999)).ThrowsAsync(new Exception("Listing case with ID 999 not found."));

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => listingCaseService.DeleteListingCaseAsync(999));
    }

    // UpdateListingCaseStatusAsync
    [Fact]
    public async Task UpdateListingCaseStatusAsync_WhenValidTransition_ReturnUpdatedListingCase()
    {
        // Arrange
        var listingCase = new ListingCase()
        {
            Id = 1,
            Title = "Test Property",
            ListcaseStatus = ListcaseStatus.Pending
        };

        _listingCaseRepositoryMock.Setup(repo => repo.UpdateListingCaseStatusAsync(1, ListcaseStatus.Pending)).ReturnsAsync(listingCase);

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act
        var result = await listingCaseService.UpdateListingCaseStatusAsync(1, ListcaseStatus.Pending);

        // Assert
        result.Should().NotBeNull();
        result.ListcaseStatus.Should().Be(ListcaseStatus.Pending);
    }

    [Fact]
    public async Task UpdateListingCaseStatusAsync_WhenNotExists_ThrowException()
    {
        // Arrange
        _listingCaseRepositoryMock.Setup(repo => repo.UpdateListingCaseStatusAsync(999, ListcaseStatus.Pending)).ThrowsAsync(new Exception("Listing case with ID 999 not found."));

        var listingCaseService = new ListingCaseService(_listingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => listingCaseService.UpdateListingCaseStatusAsync(999, ListcaseStatus.Pending));
    }
}