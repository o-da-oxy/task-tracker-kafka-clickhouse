namespace DddService.Aggregates;

public class ProjectId
{
    public Guid Value { get; }

    private ProjectId(Guid value)
    {
        Value = value;
    }

    public static ProjectId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidProjectIdException(value);
        }

        return new ProjectId(value);
    }

    public static implicit operator Guid(ProjectId projectId)
    {
        return projectId.Value;
    }
}

public class CommandId
{
    public Guid Value { get; }

    private CommandId(Guid value)
    {
        Value = value;
    }

    public static CommandId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidCommandIdException(value);
        }

        return new CommandId(value);
    }

    public static implicit operator Guid(CommandId commandId)
    {
        return commandId.Value;
    }
}

public class TaskId
{
    public Guid Value { get; }

    private TaskId(Guid value)
    {
        Value = value;
    }

    public static TaskId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidTaskIdException(value);
        }

        return new TaskId(value);
    }

    public static implicit operator Guid(TaskId taskId)
    {
        return taskId.Value;
    }
}

public class SprintId
{
    public Guid Value { get; }

    private SprintId(Guid value)
    {
        Value = value;
    }

    public static SprintId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidSprintIdException(value);
        }

        return new SprintId(value);
    }

    public static implicit operator Guid(SprintId sprintId)
    {
        return sprintId.Value;
    }
}

public class DeveloperId
{
    public Guid Value { get; }

    private DeveloperId(Guid value)
    {
        Value = value;
    }

    public static DeveloperId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidDeveloperIdException(value);
        }

        return new DeveloperId(value);
    }

    public static implicit operator Guid(DeveloperId developerId)
    {
        return developerId.Value;
    }
}

public enum DeveloperLevel
{
    Junior,
    Middle,
    Senior
}