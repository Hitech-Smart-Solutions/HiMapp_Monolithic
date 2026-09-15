using MediatR;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;

public sealed class GetDailyDepartmentalLabourSlipByIdAndProgramId : IRequest<GetDailyDepartmentalLabourSlipByIdModel?>
{
    public int Id { get; }
    public int ProgramId { get; }

    public GetDailyDepartmentalLabourSlipByIdAndProgramId(int id, int programId)
    {
        Id = id;
        ProgramId = programId;
    }
}
