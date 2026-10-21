using Himapp.PM.Application.Features.WorkDescriptions.Models;
using MediatR;

namespace Himapp.PM.Application.Features.WorkDescriptions.Queries;

public sealed record GetAllWorkDescriptionsQuery() : IRequest<IReadOnlyCollection<WorkDescriptionDto>>;
