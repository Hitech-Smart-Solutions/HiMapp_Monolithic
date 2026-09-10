using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Queries;

public sealed record GetActivityDuplicateQuery(int CompanyID, string ActivityName, int? ExcludeActivityId = null) : IRequest<bool>;
