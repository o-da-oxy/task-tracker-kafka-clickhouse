using DddService.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features;

public record GetAllProjectsQuery : IRequest<IList<ProjectDto>>;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IList<ProjectDto>>
{
    private readonly ProjectAggregateDbContext _db;

    public GetAllProjectsQueryHandler(ProjectAggregateDbContext db)
    {
        _db = db;
    }

    public async Task<IList<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Projects.AsNoTrackingWithIdentityResolution()
        .Select(p => new ProjectDto(p.Id.Value.ToString(), p.Name))
        .ToListAsync();
    }
}
