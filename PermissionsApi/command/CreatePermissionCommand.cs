using Nest;
using PermissionsApi.Mapper;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;
using PermissionsApi.Services;

namespace PermissionsApi.command
{
    public class CreatePermissionCommand : ICommand
    {
        public CreatePermissionDto? PermissionDto { get; set; }
    }

    public class CreatePermissionCommandHandler(IUnitOfWork unitOfWork, IElasticClient elasticClient, KafkaProducerService kafkaProducerService) : ICommandHandler<CreatePermissionCommand>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IElasticClient _elasticClient = elasticClient;
        private readonly KafkaProducerService _kafkaProducerService = kafkaProducerService;

        public async Task HandleAsync(CreatePermissionCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var permission = command.PermissionDto != null ? PermissionMapper.FromDto(command.PermissionDto) : null;
            if (permission != null)
            {
                await _unitOfWork.PermissionRepository.AddPermissionAsync(permission);
                await _unitOfWork.CompleteAsync();
            }

            // Registrar evento en Elasticsearch
            var response = await _elasticClient.IndexDocumentAsync(new
            {
                EventType = "RequestPermission",
                Timestamp = DateTime.UtcNow,
                Permission = permission
            });

            if (!response.IsValid)
            {
                throw new Exception("Failed to index permission event in Elasticsearch");
            }

            // Enviar mensaje a Kafka
            var kafkaMessage = new
            {
                Id = Guid.NewGuid(),
                NameOperation = "request"
            };
            var messageValue = Newtonsoft.Json.JsonConvert.SerializeObject(kafkaMessage);
            await _kafkaProducerService.ProduceMessageAsync("operations", kafkaMessage.Id.ToString(), messageValue);
        }
    }

}