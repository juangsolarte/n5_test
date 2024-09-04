
using Nest;
using PermissionsApi.Mapper;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;
using PermissionsApi.Services;

namespace PermissionsApi.command
{
    public class GetPermissionsQuery : IQuery<IEnumerable<PermissionDto>>
    {
    }

    public class GetPermissionsQueryHandler(IUnitOfWork unitOfWork, IElasticClient elasticClient, KafkaProducerService kafkaProducerService) : IQueryHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IElasticClient _elasticClient = elasticClient;
        private readonly KafkaProducerService _kafkaProducerService = kafkaProducerService;

        public async Task<IEnumerable<PermissionDto>> HandleAsync(GetPermissionsQuery query)
        {
            var permissions = await _unitOfWork.PermissionRepository.GetPermissionsAsync();

            // Registrar evento en Elasticsearch
            var response = await _elasticClient.IndexDocumentAsync(new
            {
                EventType = "GetPermissions",
                Timestamp = DateTime.UtcNow,
                RetrievedCount = permissions.Count(),
                Permissions = permissions
            });

            if (!response.IsValid)
            {
                throw new Exception("Failed to index permission event in Elasticsearch");
            }

            // Enviar mensaje a Kafka
            var kafkaMessage = new
            {
                Id = Guid.NewGuid(),
                NameOperation = "get"
            };
            await _kafkaProducerService.ProduceMessageAsync("operations", kafkaMessage.Id.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(kafkaMessage));


            return permissions.Select(PermissionMapper.ToDto).ToList();
        }
    }

}