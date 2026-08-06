using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetProjectActivitiesByProjectIdQuery(int ProjectId) : IRequest<DataSet>;
