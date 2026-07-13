using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Commands;

public sealed record CreateLabourCommand(
    long ProjectId,
    long ContractorId,
    string Name,
    DateOnly DateOfBirth,
    string AadhaarNumber,
    string? Pan,
    UploadedFileInfo Photo) : IRequest<LabourDto>;
