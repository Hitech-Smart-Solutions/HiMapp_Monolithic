using Himapp.PM.Application.Features.SpecificationMaster.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Queries;

public sealed record GetAllSpecificationMastersQuery : IRequest<IReadOnlyCollection<SpecificationMasterDto>>;
