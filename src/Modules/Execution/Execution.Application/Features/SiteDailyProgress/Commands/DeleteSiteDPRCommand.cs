using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Commands
{
    public sealed record DeleteSiteDPRCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
}
