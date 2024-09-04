using Moq;
using PermissionsApi.Repositories;
using PermissionsApi.command;
using PermissionsApi.Models.Entities;

public class GetPermissionTypesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetPermissionTypesQueryHandler _handler;

    public GetPermissionTypesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetPermissionTypesQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnPermissionTypeDtos()
    {
        // Arrange
        var permissionTypes = new List<PermissionType>
        {
            new PermissionType { Id = 1, Description = "Type 1" },
            new PermissionType { Id = 2, Description = "Type 2" }
        };

        _unitOfWorkMock.Setup(u => u.PermissionTypeRepository.GetPermissionTypes())
            .ReturnsAsync(permissionTypes);

        var query = new GetPermissionTypesQuery();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Type 1", result.First().Description);
    }
}
