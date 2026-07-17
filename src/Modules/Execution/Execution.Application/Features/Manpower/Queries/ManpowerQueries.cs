using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;

namespace Himapp.Execution.Application.Features.Manpower.Queries;

public sealed record GetAllManpowersQuery : IRequest<IReadOnlyCollection<ManpowerModel>>;
public sealed record GetManpowerByIdQuery(long Id) : IRequest<ManpowerModel?>;
