using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralUserRoleMappingDetails
{
    public Guid UniqueID { get; set; }

    public int ID { get; set; }

    public int CentralRoleMappingID { get; set; }

    public int UserID { get; set; }

    public int ProjectID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation Property
    public virtual CentralUserRoleMapping? CentralUserRoleMapping { get; set; }
}
