using Himapp.PM.Application.Features.SpecificationMaster.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Queries;

public sealed record GetSpecificationMasterByIdQuery(int Id) : IRequest<SpecificationMasterDto?>;
