using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities;

public class EquipmentPreventiveMaintenancePoints : BaseEntity
{
    public int? EquipmentID { get; set; }
    public int? MaintenancePointID { get; set; }
    public int? ItemID { get; set; }
    public int? ReadingTypeID { get; set; }
    public decimal? IntervalValue { get; set; }
    public decimal? TolerenceValue { get; set; }
    public int? Months { get; set; }
    public string? Remarks { get; set; }
    public int StatusID { get; set; }
    public bool IsActive { get; set; } = true;

}
