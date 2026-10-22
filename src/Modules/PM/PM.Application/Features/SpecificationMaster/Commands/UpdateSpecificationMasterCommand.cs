using Himapp.PM.Application.Features.SpecificationMaster.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Commands;

public sealed record UpdateSpecificationMasterCommand(
    int Id,
    string SpecificationName,
    bool IsActive,
    SpecificationType Type,
    int LastModifiedBy) : IRequest<SpecificationMasterDto?>;
