using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities;

public class EquipmentMeterCumulativeDetail : BaseEntity
{
    public int? EquipmentID { get; set; }
    public decimal? FinalReadingAmount { get; set; }
    public decimal? ActualOpeningReadingAmount { get; set; }
    public int? StatusID { get; set; }
    public bool IsActive { get; set; } = true;
}