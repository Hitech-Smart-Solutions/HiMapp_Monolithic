namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed class PlanningSectionModel
{
    public int Id { get; init; }
    public string SectionName { get; init; } = string.Empty;
    public string LabelName { get; init; } = string.Empty;

    public PlanningSectionModel(int id, string sectionName, string labelName)
    {
        Id = id;
        SectionName = sectionName;
        LabelName = labelName;
    }
}
