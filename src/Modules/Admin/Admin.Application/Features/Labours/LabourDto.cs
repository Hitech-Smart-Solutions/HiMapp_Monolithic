namespace Himapp.Admin.Application.Features.Labours;

public sealed record LabourDto(
    long Id,
    long ProjectId,
    long ContractorId,
    string Name,
    DateOnly DateOfBirth,
    string AadhaarMasked,
    string? Pan,
    long PhotoFileId,
    string Status);
