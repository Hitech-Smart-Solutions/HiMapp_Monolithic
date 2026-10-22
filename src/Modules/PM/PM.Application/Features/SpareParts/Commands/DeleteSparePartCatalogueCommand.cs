using MediatR;

namespace Himapp.PM.Application.Features.SpareParts.Commands;

public sealed record DeleteSparePartCatalogueCommand(int Id) : IRequest<bool>;
