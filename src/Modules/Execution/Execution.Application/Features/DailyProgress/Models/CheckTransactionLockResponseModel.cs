using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models
{
    public class CheckTransactionLockResponseModel
    {
        public bool IsLocked { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
