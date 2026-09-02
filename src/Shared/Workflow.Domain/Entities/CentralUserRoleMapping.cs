using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralUserRoleMapping : BaseEntity
{
    public CentralUserRoleMapping()
    {
        Details = new HashSet<CentralUserRoleMappingDetails>();
    }

    public string RoleCode { get; set; } = string.Empty;

    public string? RoleName { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public virtual ICollection<CentralUserRoleMappingDetails>? Details { get; set; }
}