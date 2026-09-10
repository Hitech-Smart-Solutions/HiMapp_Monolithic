using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public class PlanningDocumentDetail : BaseEntity
    {

        public int PlanningID { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public virtual Planning? Planning { get; set; }
    }
}
