using Himapp.PM.Application.Features.SpecificationMaster.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Commands;

public sealed record CreateSpecificationMasterCommand(
    string SpecificationName,
    bool IsActive,
    SpecificationType Type,
    int CreateBy,
    int LastModifiedBy) : IRequest<SpecificationMasterDto>;
