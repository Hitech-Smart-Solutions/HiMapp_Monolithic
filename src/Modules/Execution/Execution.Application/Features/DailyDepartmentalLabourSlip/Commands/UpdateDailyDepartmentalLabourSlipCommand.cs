using MediatR;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;

public sealed class UpdateDailyDepartmentalLabourSlipCommand : IRequest<DailyDepartmentalLabourSlipModel?>
{
    public int Id { get; }
    public UpdateDailyDepartmentalLabourSlipRequest Request { get; }
    public UpdateDailyDepartmentalLabourSlipCommand(int id, UpdateDailyDepartmentalLabourSlipRequest request)
    {
        Id = id;
        Request = request;
    }
}
