using MediatR;
using System.Data;

namespace Himapp.PM.Application.Features.SpareParts.Queries;

public sealed record GetAllSparePartCataloguesQuery() : IRequest<List<object>>;
