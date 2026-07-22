using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Commands;

public sealed record CreateActivityCommand(int CompanyID, string ActivityName,int UOMID,int CreateBy,int LastModifiedBy) : IRequest<ActivityDto>;
public sealed record UpdateActivityCommand(long Id, string ActivityName, int UOMID, int LastModifiedBy) : IRequest<ActivityDto?>;
public sealed record DeleteActivityCommand(long Id) : IRequest<bool>;
