namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class SectionWiseHindranceModel
{
    public int SectionID { get; set; }
    public string? Hindrance { get; set; }

    public SectionWiseHindranceModel(int sectionId, string? hindrance)
    {
        SectionID = sectionId;
        Hindrance = hindrance;
    }
}
