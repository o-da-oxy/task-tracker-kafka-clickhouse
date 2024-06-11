using DddService.Aggregates.Events;
using MediatR;

namespace DddService.Handlers;

public class ProjectCreatedDomainEventHandler: INotificationHandler<ProjectCreatedDomainEvent>
{
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;

    public ProjectCreatedDomainEventHandler(ILogger<ProjectCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"ВСЕМ ПРИВЕТ! NEW PROJECT CREATED {DateTime.Now}: ID={notification.ProjectId}, ProjectName={notification.ProjectName}");

        return Task.CompletedTask;
    }
}