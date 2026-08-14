using MediatR;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Commands;

public sealed record CreateCentralUserRoleMappingCommand(CreateCentralUserRoleMappingRequest Request) : IRequest<CentralUserRoleMappingDto>;
public sealed record UpdateCentralUserRoleMappingCommand(int Id, UpdateCentralUserRoleMappingRequest Request) : IRequest<CentralUserRoleMappingDto?>;
public sealed record DeleteCentralUserRoleMappingCommand(int Id, AddTransactionActionHistoryDTO actionHistory) : IRequest<bool>;
