using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsSyncLogs.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsSyncLogs.Queries;

public sealed record GetDynamicsSyncLogsQuery(ListSearchParams Params) : IRequest<PagedResult<DynamicsSyncLogDto>>;
