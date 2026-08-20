using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities;

public class DailyProgressHindrance
{
    public DailyProgressHindrance()
    {

    }

    public int ID { get; set; }

    public Guid UniqueID { get; set; }

    public int DailyProgressID { get; set; }

    public string? Hindrance { get; set; }

    public string? AudioUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }

    public virtual DailyProgress? DailyProgress { get; set; }
}
