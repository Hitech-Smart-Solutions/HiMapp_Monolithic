using MediatR;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;

public sealed class GetDailyDepartmentalLabourSlipByIdQuery : IRequest<DailyDepartmentalLabourSlipDto?>
{
    public int Id { get; }
    public GetDailyDepartmentalLabourSlipByIdQuery(int id) => Id = id;
}
