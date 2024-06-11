using DddService.Common;

namespace DddService.Aggregates.Events;
public record ProjectCreatedDomainEvent(Guid ProjectId, string ProjectName) : IDomainEvent;
