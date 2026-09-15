using Himapp.Execution.Application.Features.DailyProgress.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries;

public sealed record GetSectionWiseHindrancesByProjectQuery(int ProjectID, DateOnly ReportDate) : IRequest<List<SectionWiseHindranceModel>>;
