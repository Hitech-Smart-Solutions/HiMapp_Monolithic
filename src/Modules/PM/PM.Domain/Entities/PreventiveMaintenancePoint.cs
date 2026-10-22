using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities;

public class PreventiveMaintenancePoint : BaseEntity
{
    public string? MaintenancePointName { get; set; }
    public int StatusID { get; set; }
    public bool IsActive { get; set; } = true;
}
