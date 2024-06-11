using System.Text.Json;
using DddService.Aggregates.Events;
using DddService.EventBus;
using MediatR;
using Message;

namespace DddService.Handlers;

public class ProjectCreatedDomainEventHandler: INotificationHandler<ProjectCreatedDomainEvent>
{
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;
    private readonly KafkaProducerService _kafkaProducerService;

    public ProjectCreatedDomainEventHandler(
        ILogger<ProjectCreatedDomainEventHandler> logger,
        KafkaProducerService kafkaProducerService)
    {
        _logger = logger;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"ВСЕМ ПРИВЕТ! NEW PROJECT CREATED {DateTime.Now}: ID={notification.ProjectId}, ProjectName={notification.ProjectName}");

        var projectCreatedMessage = new ProjectCreatedMessage(notification.ProjectId, notification.ProjectName, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());
        await _kafkaProducerService.ProduceAsync("NewProjectsCreated", JsonSerializer.Serialize(projectCreatedMessage));
    }
}