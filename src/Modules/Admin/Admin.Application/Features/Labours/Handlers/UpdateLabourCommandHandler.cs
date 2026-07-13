using System.Security.Cryptography;
using System.Text;
using Himapp.Admin.Application.Features.Labours.Commands;
using Himapp.Files.Services;
using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Handlers;

internal sealed class UpdateLabourCommandHandler : IRequestHandler<UpdateLabourCommand, LabourDto?>
{
    private readonly ILabourRepository _repository;
    private readonly IFileService _fileService;

    public UpdateLabourCommandHandler(ILabourRepository repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<LabourDto?> Handle(UpdateLabourCommand request, CancellationToken cancellationToken)
    {
        var labour = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (labour is null)
        {
            return null;
        }

        var photoFileId = labour.PhotoFileId;
        if (request.Photo is not null)
        {
            var photo = await _fileService.RegisterAsync(
                request.Photo.FileName,
                request.Photo.ContentType,
                "admin/labour-photo",
                request.Photo.SizeBytes,
                cancellationToken);

            photoFileId = photo.Id;
        }

        labour.UpdateProfile(
            request.ProjectId,
            request.ContractorId,
            request.Name.Trim(),
            request.DateOfBirth,
            MaskAadhaar(request.AadhaarNumber),
            HashAadhaar(request.AadhaarNumber),
            request.Pan?.Trim(),
            photoFileId);

        await _repository.UpdateAsync(labour, cancellationToken);
        return labour.ToDto();
    }

    private static string MaskAadhaar(string aadhaarNumber) =>
        aadhaarNumber.Length <= 4 ? aadhaarNumber : $"XXXX-XXXX-{aadhaarNumber[^4..]}";

    private static byte[] HashAadhaar(string aadhaarNumber) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(aadhaarNumber));
}
