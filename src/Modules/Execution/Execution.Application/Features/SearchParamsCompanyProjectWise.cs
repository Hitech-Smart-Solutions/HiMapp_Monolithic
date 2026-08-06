using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features
{
    public class SearchParamsCompanyProjectWise
    {
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public string? FilterColumn { get; set; } = "";
        public string? FilterValue { get; set; } = "";
        public string SortColumn { get; set; } = "";
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string IsActive { get; set; } = "";
    }
}
