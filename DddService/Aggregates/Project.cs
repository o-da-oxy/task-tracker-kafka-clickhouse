using DddService.Common;
using DddService.Aggregates.Events;

namespace DddService.Aggregates;

public class Project : Aggregate<ProjectId>
{
    public string Name { get; private set; } = default!;
    public IList<Command> Commands { get; private set; } = new List<Command>();
    public IList<Sprint> Sprints { get; private set; } = new List<Sprint>();
    public IList<Developer> Developers { get; private set; } = new List<Developer>();

    public static Project Create(ProjectId id, string name, bool isDeleted = false)
    {
        var project = new Project
        {
            Id = id,
            Name = name
        };

        var @event = new ProjectCreatedDomainEvent(project.Id, project.Name);
        project.AddDomainEvent(@event);

        return project;
    }

    public void AddCommand(Command command)
    {
        Commands.Add(command);
    }

    public void AddSprint(Sprint sprint)
    {
        Sprints.Add(sprint);
    }

    public void AddDeveloper(Developer developer)
    {
        Developers.Add(developer);
    }
}

public class Command : Entity<CommandId>
{
    public string Name { get; private set; } = default!;
    public IList<Tasks> Tasks { get; private set; } = new List<Tasks>();

    public static Command Create(CommandId id, string name, bool isDeleted = false)
    {
        var command = new Command
        {
            Id = id,
            Name = name
        };

        return command;
    }

    public void AddTask(Tasks task)
    {
        Tasks.Add(task);
    }
}

public class Tasks : Entity<TaskId>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public Sprint? Sprint { get; private set; }
    public Developer? Assignee { get; private set; }

    public static Tasks Create(TaskId id, string name, string description, bool isDeleted = false)
    {
        var task = new Tasks
        {
            Id = id,
            Name = name,
            Description = description
        };

        return task;
    }

    public void AssignToSprint(Sprint sprint)
    {
        Sprint = sprint;
    }

    public void AssignToDeveloper(Developer developer)
    {
        Assignee = developer;
    }
}


public class Sprint : Entity<SprintId>
{
    public string Name { get; private set; } = default!;

    public IList<Tasks> Tasks { get; private set; } = new List<Tasks>();

    public static Sprint Create(SprintId id, string name, bool isDeleted = false)
    {
        var sprint = new Sprint
        {
            Id = id,
            Name = name
        };

        return sprint;
    }

    public void AddTask(Tasks task)
    {
        Tasks.Add(task);
    }
}

public class Developer : Entity<DeveloperId>
{
    public string Name { get; private set; } = default!;
    public DeveloperLevel Level { get; private set; } = default!;
    public IList<Tasks> AssignedTasks { get; private set; } = new List<Tasks>();

    public static Developer Create(DeveloperId id, string name, DeveloperLevel level, bool isDeleted = false)
    {
        var developer = new Developer
        {
            Id = id,
            Name = name,
            Level = level
        };

        return developer;
    }

    public void AssignTask(Tasks task)
    {
        AssignedTasks.Add(task);
    }
}
