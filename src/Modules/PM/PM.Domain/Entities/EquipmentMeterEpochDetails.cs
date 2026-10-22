using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities;

public class EquipmentMeterEpochDetails : BaseEntity
{
    public int? EquipmentID { get; set; }
    public DateTime? OpenningDate { get; set; }
    public int? MeterReadingID { get; set; }
    public decimal? ResetReadingAmount { get; set; }
    public decimal? OpeningReadingAmount { get; set; }
    public int? ReasonID { get; set; }
    public string? Remarks { get; set; }
    public int? StatusID { get; set; }
    public bool IsActive { get; set; } = true;
}