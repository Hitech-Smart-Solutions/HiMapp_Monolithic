using MediatR;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;

public sealed class CreateDailyDepartmentalLabourSlipCommand : IRequest<DailyDepartmentalLabourSlipModel>
{
    public CreateDailyDepartmentalLabourSlipRequest Request { get; }
    public CreateDailyDepartmentalLabourSlipCommand(CreateDailyDepartmentalLabourSlipRequest request) => Request = request;
}
