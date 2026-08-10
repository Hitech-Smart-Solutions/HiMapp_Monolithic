using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public sealed class ActivityCategoryDetails
    {
        public int ID { get; set; }
        public Guid UniqueID { get; set; }
        public int ProjectID { get; set; }
        public int ActivityID { get; set; }
        public int CategoryTypeID { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}
