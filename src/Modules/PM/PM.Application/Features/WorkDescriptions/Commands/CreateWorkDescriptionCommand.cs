using Himapp.PM.Application.Features.WorkDescriptions.Models;
using MediatR;

namespace Himapp.PM.Application.Features.WorkDescriptions.Commands;

public sealed record CreateWorkDescriptionCommand(string WorkDescriptionName, bool IsActive, int CreateBy, int LastModifiedBy) : IRequest<WorkDescriptionDto>;
