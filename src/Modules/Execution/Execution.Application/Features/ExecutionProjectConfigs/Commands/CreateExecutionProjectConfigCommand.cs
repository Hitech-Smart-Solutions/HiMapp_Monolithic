using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.ExecutionProjectConfigs.Commands;

public sealed record CreateExecutionProjectConfigCommand(CreateExecutionProjectConfigRequest Request)
    : IRequest<ExecutionProjectConfigModel>;
