using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.ExecutionProjectConfigs.Queries;

public sealed record GetExecutionProjectConfigByProjectIdQuery(int ProjectId)
    : IRequest<ExecutionProjectConfigModel?>;
