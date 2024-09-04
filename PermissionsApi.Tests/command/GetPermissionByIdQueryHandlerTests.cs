using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using PermissionsApi.Repositories;
using PermissionsApi.command;
using PermissionsApi.Models.Entities;

public class GetPermissionByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetPermissionByIdQueryHandler _handler;

    public GetPermissionByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetPermissionByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnPermissionDto_When_PermissionExists()
    {
        // Arrange
        var permission = new Permission
        {
            Id = 1,
            EmployeeForename = "John",
            EmployeeSurname = "Doe",
            PermissionTypeId = 1
        };
        var permissionType = new PermissionType
        {
            Id = 1,
            Description = "Type 1"
        };

        _unitOfWorkMock.Setup(u => u.PermissionRepository.GetPermissionByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(permission);

        _unitOfWorkMock.Setup(u => u.PermissionTypeRepository.GetPermissionTypes())
            .ReturnsAsync(new List<PermissionType> { permissionType });

        // Act
        var result = await _handler.HandleAsync(new GetPermissionByIdQuery(1));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(permission.Id, result.Id);
        Assert.Equal(permission.EmployeeForename, result.EmployeeForename);
        Assert.Equal(permission.EmployeeSurname, result.EmployeeSurname);
        Assert.Equal(permissionType.Description, result.PermissionType.Description);

        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowException_When_PermissionNotFound()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.PermissionRepository.GetPermissionByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Permission?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(new GetPermissionByIdQuery(1)));
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowException_When_PermissionTypeNotFound()
    {
        // Arrange
        var permission = new Permission
        {
            Id = 1,
            EmployeeForename = "John",
            EmployeeSurname = "Doe",
            PermissionTypeId = 1
        };

        _unitOfWorkMock.Setup(u => u.PermissionRepository.GetPermissionByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(permission);

        _unitOfWorkMock.Setup(u => u.PermissionTypeRepository.GetPermissionTypes())
            .ReturnsAsync(new List<PermissionType>()); // No se devuelve ningún tipo de permiso

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(new GetPermissionByIdQuery(1)));

        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
