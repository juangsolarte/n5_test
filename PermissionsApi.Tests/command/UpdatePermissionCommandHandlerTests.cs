using Moq;
using Nest;
using PermissionsApi.command;
using PermissionsApi.Models.Dto;
using PermissionsApi.Models.Entities;
using PermissionsApi.Repositories;
using PermissionsApi.Services;
using Xunit;

namespace PermissionsApi.Tests.command
{
    public class UpdatePermissionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<KafkaProducerService> _kafkaProducerServiceMock;
        private readonly UpdatePermissionCommandHandler _handler;

        public UpdatePermissionCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _elasticClientMock = new Mock<IElasticClient>();
            _kafkaProducerServiceMock = new Mock<KafkaProducerService>();
            _handler = new UpdatePermissionCommandHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowException_When_PermissionTypeId_IsInvalid()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = 1,
                PermissionDto = new CreatePermissionDto
                {
                    EmployeeForename = "John",
                    EmployeeSurname = "Doe",
                    PermissionTypeId = 999,  // ID inválido
                    PermissionDate = DateTime.UtcNow
                }
            };

            _unitOfWorkMock.Setup(uow => uow.PermissionTypeRepository.ExistsAsync(command.PermissionDto.PermissionTypeId))
                .ReturnsAsync(false);

            var handler = new UpdatePermissionCommandHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowException_When_PermissionId_IsInvalid()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = 999,  // ID inválido
                PermissionDto = new CreatePermissionDto
                {
                    EmployeeForename = "John",
                    EmployeeSurname = "Doe",
                    PermissionTypeId = 1,
                    PermissionDate = DateTime.UtcNow
                }
            };

            _unitOfWorkMock.Setup(uow => uow.PermissionTypeRepository.ExistsAsync(command.PermissionDto.PermissionTypeId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.PermissionRepository.PermissionExistsAsync(command.Id))
                .ReturnsAsync(false);

            var handler = new UpdatePermissionCommandHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async Task HandleAsync_Should_UpdatePermission_And_SendMessageToKafka()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = 1,
                PermissionDto = new CreatePermissionDto
                {
                    EmployeeForename = "John",
                    EmployeeSurname = "Doe",
                    PermissionTypeId = 1,
                    PermissionDate = DateTime.UtcNow
                }
            };

            _unitOfWorkMock.Setup(uow => uow.PermissionTypeRepository.ExistsAsync(command.PermissionDto.PermissionTypeId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.PermissionRepository.PermissionExistsAsync(command.Id))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.PermissionRepository.UpdatePermissionAsync(It.IsAny<Permission>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .ReturnsAsync(1);

            var validIndexResponse = new Mock<IndexResponse>();
            validIndexResponse.Setup(r => r.IsValid).Returns(true);

            _elasticClientMock.Setup(e => e.IndexDocumentAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validIndexResponse.Object);

            _kafkaProducerServiceMock.Setup(k => k.ProduceMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdatePermissionCommandHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);

            // Act
            await handler.HandleAsync(command);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.PermissionRepository.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
            _elasticClientMock.Verify(e => e.IndexDocumentAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
            _kafkaProducerServiceMock.Verify(k => k.ProduceMessageAsync("operations", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowException_When_ElasticSearchFails()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = 1,
                PermissionDto = new CreatePermissionDto
                {
                    EmployeeForename = "John",
                    EmployeeSurname = "Doe",
                    PermissionTypeId = 1,
                    PermissionDate = DateTime.UtcNow
                }
            };

            _unitOfWorkMock.Setup(uow => uow.PermissionTypeRepository.ExistsAsync(command.PermissionDto.PermissionTypeId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.PermissionRepository.PermissionExistsAsync(command.Id))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.PermissionRepository.UpdatePermissionAsync(It.IsAny<Permission>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .ReturnsAsync(1);

            var invalidIndexResponse = new Mock<IndexResponse>();
            invalidIndexResponse.Setup(r => r.IsValid).Returns(false);

            _elasticClientMock.Setup(e => e.IndexDocumentAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidIndexResponse.Object);

            var handler = new UpdatePermissionCommandHandler(_unitOfWorkMock.Object, _elasticClientMock.Object, _kafkaProducerServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(command));
        }
    }

}