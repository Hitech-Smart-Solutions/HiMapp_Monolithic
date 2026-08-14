using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralUserRoleMapping
{
    public CentralUserRoleMapping()
    {
        Details = new HashSet<CentralUserRoleMappingDetails>();
    }

    public Guid UniqueID { get; set; }

    public int ID { get; set; }

    public string RoleCode { get; set; } = string.Empty;

    public string? RoleName { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation Property
    public virtual ICollection<CentralUserRoleMappingDetails>? Details { get; set; }
}