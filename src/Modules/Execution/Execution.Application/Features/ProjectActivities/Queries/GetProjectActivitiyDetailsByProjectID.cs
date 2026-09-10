using Himapp.Execution.Application.Features.ProjectActivities.Models;
using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetProjectActivitiyDetailsByProjectID(int ProjectId) : IRequest<List<ProjectActivityCategoryDetailsModel>>;
