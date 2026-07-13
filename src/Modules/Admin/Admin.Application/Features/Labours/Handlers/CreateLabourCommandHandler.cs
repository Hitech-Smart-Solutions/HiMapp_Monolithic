using System.Security.Cryptography;
using System.Text;
using Himapp.Admin.Application.Features.Labours.Commands;
using Himapp.Files.Services;
using MediatR;
using LabourEntity = Himapp.Admin.Domain.Labour.Labour;

namespace Himapp.Admin.Application.Features.Labours.Handlers;

internal sealed class CreateLabourCommandHandler : IRequestHandler<CreateLabourCommand, LabourDto>
{
    private readonly ILabourRepository _repository;
    private readonly IFileService _fileService;

    public CreateLabourCommandHandler(ILabourRepository repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<LabourDto> Handle(CreateLabourCommand request, CancellationToken cancellationToken)
    {
        var photo = await _fileService.RegisterAsync(
            request.Photo.FileName,
            request.Photo.ContentType,
            "admin/labour-photo",
            request.Photo.SizeBytes,
            cancellationToken);

        var labour = LabourEntity.Register(
            request.ProjectId,
            request.ContractorId,
            request.Name.Trim(),
            request.DateOfBirth,
            MaskAadhaar(request.AadhaarNumber),
            HashAadhaar(request.AadhaarNumber),
            request.Pan?.Trim(),
            photo.Id);

        await _repository.AddAsync(labour, cancellationToken);
        return labour.ToDto();
    }

    private static string MaskAadhaar(string aadhaarNumber) =>
        aadhaarNumber.Length <= 4 ? aadhaarNumber : $"XXXX-XXXX-{aadhaarNumber[^4..]}";

    private static byte[] HashAadhaar(string aadhaarNumber) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(aadhaarNumber));
}
