using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public class PlanningDocumentDetail
    {
        public int ID { get; set; }
        public Guid UniqueID { get; set; }

        public int PlanningID { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual Planning? Planning { get; set; }
    }
}
