using Himapp.Execution.Application.Features.Activities.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Commands;

public sealed record CreateActivityCommand(int CompanyID, string ActivityName, int UOMID, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate, bool OutputRequired, int CreateBy, int LastModifiedBy) : IRequest<ActivityDto>;
