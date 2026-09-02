using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralUserRoleMappingDetails : BaseEntity
{

    public int CentralRoleMappingID { get; set; }

    public int UserID { get; set; }

    public int ProjectID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public virtual CentralUserRoleMapping? CentralUserRoleMapping { get; set; }
}
