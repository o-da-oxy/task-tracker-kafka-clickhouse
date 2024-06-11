namespace DddService.Aggregates;

public record CommandDto(string Id, string Name, IList<TaskDto> Tasks);
public record SprintDto(string Id, string Name, IList<TaskDto> Tasks);
public record TaskDto(string Id, string Name, string Description);
public record DeveloperDto(string Id, string Name, DeveloperLevel Level, IList<TaskDto> AssignedTasks);
public record ProjectDto(string Id, string Name);
public record ProjectInputModel(string Name);