using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities;

public class DailyProgressHindrance : BaseEntity
{
    public DailyProgressHindrance()
    {

    }

    public int DailyProgressID { get; set; }

    public string? Hindrance { get; set; }

    public string? AudioUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual DailyProgress? DailyProgress { get; set; }
}
