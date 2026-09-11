namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class SectionWisePhotoModel
{
    public int SectionID { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Caption { get; set; }
    public string? FileName { get; set; }

    public SectionWisePhotoModel(int sectionId, string? photoUrl, string? caption, string? fileName)
    {
        SectionID = sectionId;
        PhotoUrl = photoUrl;
        Caption = caption;
        FileName = fileName;
    }
}
