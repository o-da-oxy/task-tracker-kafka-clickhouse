using System.Net;
using DddService.Common;

namespace DddService.Aggregates;

public class ProjectAlreadyExistException : ConflictException
{
    public ProjectAlreadyExistException(int? code = default) : base("Project already exist!", code)
    {
    }
}

public class InvalidProjectIdException : BadRequestException
{
    public InvalidProjectIdException(Guid projectId)
        : base($"ProjectId: '{projectId}' is invalid.", (int)HttpStatusCode.BadRequest)
    {
    }
}

public class InvalidCommandIdException : BadRequestException
{
    public InvalidCommandIdException(Guid commandId)
        : base($"CommandId: '{commandId}' is invalid.", (int)HttpStatusCode.BadRequest)
    {
    }
}

public class InvalidTaskIdException : BadRequestException
{
    public InvalidTaskIdException(Guid taskId)
        : base($"TaskId: '{taskId}' is invalid.", (int)HttpStatusCode.BadRequest)
    {
    }
}

public class InvalidSprintIdException : BadRequestException
{
    public InvalidSprintIdException(Guid sprintId)
        : base($"SprintId: '{sprintId}' is invalid.", (int)HttpStatusCode.BadRequest)
    {
    }
}

public class InvalidDeveloperIdException : BadRequestException
{
    public InvalidDeveloperIdException(Guid developerId)
        : base($"DeveloperId: '{developerId}' is invalid.", (int)HttpStatusCode.BadRequest)
    {
    }
}
