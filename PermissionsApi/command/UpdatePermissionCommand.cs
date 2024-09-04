using Nest;
using PermissionsApi.Mapper;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;
using PermissionsApi.Services;

namespace PermissionsApi.command
{
    public class UpdatePermissionCommand : ICommand
    {
        public int Id { get; set; }
        public CreatePermissionDto? PermissionDto { get; set; }
    }

    public class UpdatePermissionCommandHandler(IUnitOfWork unitOfWork, IElasticClient elasticClient, KafkaProducerService kafkaProducerService) : ICommandHandler<UpdatePermissionCommand>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IElasticClient _elasticClient = elasticClient;
        private readonly KafkaProducerService _kafkaProducerService = kafkaProducerService;

        public async Task HandleAsync(UpdatePermissionCommand command)
        {
            if (command.PermissionDto != null && !await _unitOfWork.PermissionTypeRepository.ExistsAsync(command.PermissionDto.PermissionTypeId))
            {
                throw new Exception("Invalid PermissionType");
            }

            if (!await _unitOfWork.PermissionRepository.PermissionExistsAsync(command.Id))
            {
                throw new Exception("Invalid Permission Id");
            }


            var permission = command.PermissionDto != null ? PermissionMapper.FromDto(command.PermissionDto) : null;
            if (permission != null)
            {
                permission.Id = command.Id;
                await _unitOfWork.PermissionRepository.UpdatePermissionAsync(permission);
            }

            await _unitOfWork.CompleteAsync();

            // Registrar evento en Elasticsearch
            var response = await _elasticClient.IndexDocumentAsync(new
            {
                EventType = "ModifyPermission",
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
                NameOperation = "modify"
            };
            var messageValue = Newtonsoft.Json.JsonConvert.SerializeObject(kafkaMessage);
            await _kafkaProducerService.ProduceMessageAsync("operations", kafkaMessage.Id.ToString(), messageValue);

        }

    }
}
