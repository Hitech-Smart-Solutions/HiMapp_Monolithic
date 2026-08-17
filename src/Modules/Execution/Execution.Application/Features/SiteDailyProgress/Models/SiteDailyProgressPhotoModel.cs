using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models
{
    public sealed class SiteDailyProgressPhotoModel
    {
        public int Id { get; init; }
        public Guid UniqueId { get; init; }
        public string PhotoUrl { get; init; } = string.Empty;
        public string? Caption { get; init; }

        public SiteDailyProgressPhotoModel(
            int id,
            Guid uniqueId,
            string photoUrl,
            string? caption)
        {
            Id = id;
            UniqueId = uniqueId;
            PhotoUrl = photoUrl;
            Caption = caption;
        }
    }
}
