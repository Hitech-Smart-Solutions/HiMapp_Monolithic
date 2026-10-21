using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities;

public class MeterReadingType : BaseEntity
{
    public string ReadingTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int StatusID { get; set; }
    public bool IsActive { get; set; } = true;
}