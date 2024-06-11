using DddService.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features;

public record CreateProjectCommand(string Name) : IRequest<CreateProjectResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record CreateProjectResult(Guid Id);

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly ProjectAggregateDbContext _db;

    public CreateProjectCommandHandler(ProjectAggregateDbContext db)
    {
        _db = db;
    }

    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var projects = await _db.Projects.AsNoTracking().ToListAsync();
        var existingProject = projects.FirstOrDefault(p => p.Name == request.Name);
        if (existingProject is not null)
        {
            throw new ProjectAlreadyExistException();
        }
        var projectEntity = _db.Projects.Add(Project.Create(ProjectId.Of(Guid.NewGuid()), request.Name)).Entity;
        await _db.SaveChangesAsync();

        return new CreateProjectResult(projectEntity.Id.Value);
    }
}
