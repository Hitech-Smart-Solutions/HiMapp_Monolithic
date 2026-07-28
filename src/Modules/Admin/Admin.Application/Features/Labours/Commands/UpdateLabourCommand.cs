using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Commands;

public sealed record UpdateLabourCommand(
    int Id,
    int ProjectId,
    int ContractorId,
    string Name,
    DateOnly DateOfBirth,
    string AadhaarNumber,
    string? Pan,
    UploadedFileInfo? Photo) : IRequest<LabourDto?>;
