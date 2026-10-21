using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Commands;

public sealed record DeleteSpecificationMasterCommand(int Id) : IRequest<bool>;
