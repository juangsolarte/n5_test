using Moq;
using Nest;
using PermissionsApi.command;
using PermissionsApi.Models.Entities;
using PermissionsApi.Repositories;
using PermissionsApi.Services;

namespace PermissionsApi.Tests.command
{
    public class GetPermissionsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<KafkaProducerService> _kafkaProducerServiceMock;
        private readonly GetPermissionsQueryHandler _handler;

        public GetPermissionsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _elasticClientMock = new Mock<IElasticClient>();
            _kafkaProducerServiceMock = new Mock<KafkaProducerService>();
            _handler = new GetPermissionsQueryHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);
        }

        [Fact]
        public async Task HandleAsync_Should_ReturnPermissions()
        {
            // Arrange
            var permissions = new List<Permission>
        {
            new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionType = new PermissionType { Description = "Type 1" }, PermissionDate = DateTime.Now },
            new Permission { Id = 2, EmployeeForename = "Jane", EmployeeSurname = "Doe", PermissionType = new PermissionType { Description = "Type 2" }, PermissionDate = DateTime.Now }
        };

            _unitOfWorkMock.Setup(u => u.PermissionRepository.GetPermissionsAsync())
                .ReturnsAsync(permissions);

            var validIndexResponse = new Mock<IndexResponse>();
            validIndexResponse.Setup(r => r.IsValid).Returns(true);

            _elasticClientMock.Setup(e => e.IndexDocumentAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validIndexResponse.Object);

            _kafkaProducerServiceMock.Setup(k => k.ProduceMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var query = new GetPermissionsQuery();

            // Act
            var result = await _handler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("John", result.First().EmployeeForename);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowException_When_ElasticSearchFails()
        {
            // Arrange
            var permissions = new List<Permission>
        {
            new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe" }
        };

            _unitOfWorkMock.Setup(u => u.PermissionRepository.GetPermissionsAsync())
                .ReturnsAsync(permissions);

            var invalidIndexResponse = new Mock<IndexResponse>();
            invalidIndexResponse.Setup(r => r.IsValid).Returns(false);

            _elasticClientMock.Setup(e => e.IndexDocumentAsync(It.IsAny<object>(), CancellationToken.None))
                .ReturnsAsync(invalidIndexResponse.Object);

            var query = new GetPermissionsQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(query));
        }
    }
}