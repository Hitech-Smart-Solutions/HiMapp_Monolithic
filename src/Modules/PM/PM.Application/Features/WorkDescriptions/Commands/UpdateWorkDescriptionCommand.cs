using Himapp.PM.Application.Features.WorkDescriptions.Models;
using MediatR;

namespace Himapp.PM.Application.Features.WorkDescriptions.Commands;

public sealed record UpdateWorkDescriptionCommand(long Id, string WorkDescriptionName, bool IsActive, int LastModifiedBy) : IRequest<WorkDescriptionDto?>;
