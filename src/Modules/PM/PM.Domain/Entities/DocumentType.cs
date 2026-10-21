using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Domain.Entities
{
    public sealed class DocumentType : BaseEntity
    {
        public string DocumentTypeName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Remarks { get; set; }
    }
}
