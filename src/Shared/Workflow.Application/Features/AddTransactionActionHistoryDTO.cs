using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features
{
    public class AddTransactionActionHistoryDTO
    {
        public int UserId { get; set; }

        public Actions Actions { get; set; }

        public int ProgramId { get; set; }

        public int ProgramRowId { get; set; }

        public int RemarksId { get; set; }

        public string Remarks { get; set; } = null!;
    }
}
