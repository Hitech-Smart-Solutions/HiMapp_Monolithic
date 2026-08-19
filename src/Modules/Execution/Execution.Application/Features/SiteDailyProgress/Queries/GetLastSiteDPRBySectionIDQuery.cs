using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries
{
    public sealed record GetLastSiteDPRBySectionIDQuery(int ProjectId,int SectionId) : IRequest<SiteDailyProgressModel?>;
}
