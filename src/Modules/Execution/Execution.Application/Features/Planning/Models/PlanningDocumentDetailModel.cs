using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.Planning.Models
{
    public sealed class PlanningDocumentDetailModel
    {
        public int Id { get; init; }
        public Guid UniqueId { get; init; }
        public string DocumentName { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public string? FileExtension { get; init; }
        public string? ContentType { get; init; }

        public PlanningDocumentDetailModel(int id, Guid uniqueId, string documentName, string fileName, string filePath, string? fileExtension, string? contentType)
        {
            Id = id;
            UniqueId = uniqueId;
            DocumentName = documentName;
            FileName = fileName;
            FilePath = filePath;
            FileExtension = fileExtension;
            ContentType = contentType;
        }
    }

    public sealed class PlanningDocumentDetailRequest
    {
        public string DocumentName { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public string? FileExtension { get; init; }
        public string? ContentType { get; init; }
    }
}
