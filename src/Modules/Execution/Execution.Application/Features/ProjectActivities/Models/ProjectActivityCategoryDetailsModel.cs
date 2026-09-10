using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.ProjectActivities.Models
{
    public sealed class ProjectActivityCategoryDetailsModel
    {
        public int? ID { get; set; }
        public int ProjectID { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Rate { get; set; }
    }
}
