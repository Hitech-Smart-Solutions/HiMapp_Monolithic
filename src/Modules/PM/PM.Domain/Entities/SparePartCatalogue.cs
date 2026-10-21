using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities
{
    public class SparePartCatalogue : BaseEntity
    {
        public int EquipmentcategoryID { get; set; }
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentPath { get; set; }
        public int StatusID { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
