using MediatR;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;

public sealed class DeleteDailyDepartmentalLabourSlipCommand : IRequest<bool>
{
    public int Id { get; }
    public DeleteDailyDepartmentalLabourSlipCommand(int id) => Id = id;
}
