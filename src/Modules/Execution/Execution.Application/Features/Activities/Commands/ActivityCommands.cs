using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Commands;

public sealed record CreateActivityCommand(int CompanyID, string ActivityName,int UOMID, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate,bool OutputRequired, int CreateBy,int LastModifiedBy) : IRequest<ActivityDto>;
public sealed record UpdateActivityCommand(int Id, string ActivityName, int UOMID,decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate,bool OutputRequired, int LastModifiedBy) : IRequest<ActivityDto?>;
public sealed record DeleteActivityCommand(int Id) : IRequest<bool>;
