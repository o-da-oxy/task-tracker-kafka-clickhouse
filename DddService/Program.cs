using Microsoft.EntityFrameworkCore;
using System.Reflection;
using DddService.Common;
using DddService.Aggregates;
using MediatR;
using DddService.Features;
using DddService.EventBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddDbContext<ProjectAggregateDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123",
        b => b.MigrationsAssembly("DddService"));
});

builder.Services.AddSingleton<KafkaProducerService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetRequiredService<ProjectAggregateDbContext>())
        {
            db.Database.EnsureCreated();
            if (!db.Projects.Any())
            {
                var project = Project.Create(ProjectId.Of(Guid.NewGuid()), "Project 1");
                var developers = new List<Developer>
                {
                    Developer.Create(DeveloperId.Of(Guid.NewGuid()), "Vasya Petrov", DeveloperLevel.Junior),
                    Developer.Create(DeveloperId.Of(Guid.NewGuid()), "Alina Ivanova", DeveloperLevel.Senior)
                };
                var commands = new List<Command>
                {
                    Command.Create(CommandId.Of(Guid.NewGuid()), "Command 1"),
                    Command.Create(CommandId.Of(Guid.NewGuid()), "Command 2")
                };
                var tasks = new List<Tasks>
                {
                    Tasks.Create(TaskId.Of(Guid.NewGuid()), "Task 1", "Task Description 1"),
                    Tasks.Create(TaskId.Of(Guid.NewGuid()), "Task 2", "Task Description 2")
                };
                var sprints = new List<Sprint>
                {
                    Sprint.Create(SprintId.Of(Guid.NewGuid()), "Sprint 1"),
                    Sprint.Create(SprintId.Of(Guid.NewGuid()), "Sprint 2")
                };

                foreach (var developer in developers)
                {
                    project.AddDeveloper(developer);
                }

                foreach (var command in commands)
                {
                    project.AddCommand(command);
                    foreach (var task in tasks)
                    {
                        command.AddTask(task);
                        task.AssignToSprint(sprints[0]);
                        task.AssignToDeveloper(developers[0]);
                    }
                }

                foreach (var sprint in sprints)
                {
                    project.AddSprint(sprint);
                    foreach (var task in tasks)
                    {
                        sprint.AddTask(task);
                    }
                }

                db.Projects.Add(project);
                db.SaveChanges();
            }
        }
    }
}

app.MapPost("api/projects", async (ProjectInputModel model, IMediator mediator) =>
{
    var command = new CreateProjectCommand(model.Name);
    var response = await mediator.Send(command);
    return Results.Created($"api/projects/{response.Id}", response);
});

app.MapGet("api/projects", async (IMediator mediator) =>
{
    return await mediator.Send(new GetAllProjectsQuery());
});

app.Run();

public class ProjectAggregateDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Command> Commands { get; set; }
    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<Developer> Developers { get; set; }

    private IMediator _mediator;

    public ProjectAggregateDbContext(DbContextOptions<ProjectAggregateDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project
        modelBuilder.Entity<Project>().HasKey(r => r.Id);
        modelBuilder.Entity<Project>().ToTable(nameof(Project));
        modelBuilder.Entity<Project>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(projectId => projectId.Value, dbId => ProjectId.Of(dbId));

        // Command
        modelBuilder.Entity<Command>().HasKey(r => r.Id);
        modelBuilder.Entity<Command>().ToTable(nameof(Command));
        modelBuilder.Entity<Command>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(commandId => commandId.Value, dbId => CommandId.Of(dbId));

        // Tasks
        modelBuilder.Entity<Tasks>().HasKey(r => r.Id);
        modelBuilder.Entity<Tasks>().ToTable(nameof(Tasks));
        modelBuilder.Entity<Tasks>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(taskId => taskId.Value, dbId => TaskId.Of(dbId));

        // Sprint
        modelBuilder.Entity<Sprint>().HasKey(r => r.Id);
        modelBuilder.Entity<Sprint>().ToTable(nameof(Sprint));
        modelBuilder.Entity<Sprint>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(sprintId => sprintId.Value, dbId => SprintId.Of(dbId));

        // Developer
        modelBuilder.Entity<Developer>().HasKey(r => r.Id);
        modelBuilder.Entity<Developer>().ToTable(nameof(Developer));
        modelBuilder.Entity<Developer>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(developerId => developerId.Value, dbId => DeveloperId.Of(dbId));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(cancellationToken);

        return result;
    }

    private async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.GetDomainEvents() != null && x.Entity.GetDomainEvents().Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }
}
