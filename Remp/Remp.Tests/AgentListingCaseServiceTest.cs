using System;
using FluentAssertions;
using Moq;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Service;

namespace Remp.Tests;

public class AgentListingCaseServiceTest
{
    private Mock<IAgentListingCaseRepository> _agentListingCaseRepositoryMock;

    public AgentListingCaseServiceTest()
    {
        _agentListingCaseRepositoryMock = new Mock<IAgentListingCaseRepository>();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenRelationNotExists_ReturnSuccess()
    {
        // Arrange
        _agentListingCaseRepositoryMock.Setup(repo => repo.RelationExistsAsync("agent-123", 1)).ReturnsAsync(false);
        _agentListingCaseRepositoryMock.Setup(repo => repo.AddAgentToListingCaseAsync(It.IsAny<AgentListingCase>()))
            .ReturnsAsync(new AgentListingCase { AgentId = "agent-123", ListingCaseId = 1 });

        var service = new AgentListingCaseService(_agentListingCaseRepositoryMock.Object);

        // Act
        var result = await service.AddAgentToListingCaseAsync("agent-123", 1);

        // Assert
        result.Should().NotBeNull();
        result.AgentId.Should().Be("agent-123");
        result.ListingCaseId.Should().Be(1);
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenRelationAlreadyExists_ThrowInvalidOperationException()
    {
        // Arrange
        _agentListingCaseRepositoryMock.Setup(repo => repo.RelationExistsAsync("agent-123", 1)).ReturnsAsync(true);

        var service = new AgentListingCaseService(_agentListingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAgentToListingCaseAsync("agent-123", 1));
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenRepositoryFails_ThrowException()
    {
        // Arrange
        _agentListingCaseRepositoryMock.Setup(repo => repo.RelationExistsAsync("agent-123", 1)).ReturnsAsync(false);
        _agentListingCaseRepositoryMock.Setup(repo => repo.AddAgentToListingCaseAsync(It.IsAny<AgentListingCase>()))
            .ThrowsAsync(new Exception("Failed to add agent to listing case."));

        var service = new AgentListingCaseService(_agentListingCaseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.AddAgentToListingCaseAsync("agent-123", 1));
    }
}
