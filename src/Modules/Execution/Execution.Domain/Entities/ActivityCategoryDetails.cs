using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public sealed class ActivityCategoryDetails : BaseEntity
    {
        public int ProjectID { get; set; }
        public int ActivityID { get; set; }
        public int CategoryTypeID { get; set; }
        public string? Name { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
    }
}
