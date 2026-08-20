using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models
{
    public sealed class SiteDailyProgressHindranceModel
    {
        public int Id { get; init; }
        public Guid UniqueId { get; init; }
        public string? Hindrance { get; init; }

        public string? AudioUrl { get; init; }

        public SiteDailyProgressHindranceModel(
            int id,
            Guid uniqueId,
            string? hindrance,
            string? audioUrl)
        {
            Id = id;
            UniqueId = uniqueId;
            Hindrance = hindrance;
            AudioUrl = audioUrl;
        }
    }
}
