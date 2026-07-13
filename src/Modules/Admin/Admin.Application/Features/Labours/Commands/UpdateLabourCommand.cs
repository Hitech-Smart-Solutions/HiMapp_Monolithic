using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Commands;

public sealed record UpdateLabourCommand(
    long Id,
    long ProjectId,
    long ContractorId,
    string Name,
    DateOnly DateOfBirth,
    string AadhaarNumber,
    string? Pan,
    UploadedFileInfo? Photo) : IRequest<LabourDto?>;
