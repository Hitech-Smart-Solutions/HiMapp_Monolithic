using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.ExecutionProjectConfigs.Commands;

public sealed record UpdateExecutionProjectConfigCommand(int Id, UpdateExecutionProjectConfigRequest Request)
    : IRequest<ExecutionProjectConfigModel?>;
