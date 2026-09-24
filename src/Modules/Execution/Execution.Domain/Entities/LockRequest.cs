using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public class LockRequest : BaseEntity
    {
        public LockRequest()
        {
            LockRequestDetails = new HashSet<LockRequestDetails>();
        }

        public string? RequestCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Remarks { get; set; }
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public int ProgramID { get; set; }
        public short StateID { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<LockRequestDetails>? LockRequestDetails { get; set; }
    }
}
