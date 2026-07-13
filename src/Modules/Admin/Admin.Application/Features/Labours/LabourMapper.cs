using LabourEntity = Himapp.Admin.Domain.Labour.Labour;

namespace Himapp.Admin.Application.Features.Labours;

internal static class LabourMapper
{
    public static LabourDto ToDto(this LabourEntity labour) =>
        new(
            labour.Id,
            labour.ProjectId,
            labour.ContractorId,
            labour.Name,
            labour.DateOfBirth,
            labour.AadhaarMasked,
            labour.Pan,
            labour.PhotoFileId,
            labour.Status.ToString());
}
