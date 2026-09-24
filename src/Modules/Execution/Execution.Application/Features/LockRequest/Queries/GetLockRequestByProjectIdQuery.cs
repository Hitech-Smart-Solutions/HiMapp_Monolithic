using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.LockRequest.Queries;

public sealed record GetLockRequestByProjectIdQuery(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<DataSet>;
