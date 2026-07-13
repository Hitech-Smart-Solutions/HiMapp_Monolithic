using Himapp.PM.Application.Features.Equipments.Commands;
using Himapp.PM.Application.Features.Equipments.Queries;
using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Handlers;

internal sealed class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, EquipmentDto>
{
    private readonly IEquipmentRepository _repository;
    public CreateEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.AddAsync(new EquipmentDto(0, request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn, "Draft"), cancellationToken);
}

internal sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, EquipmentDto?>
{
    private readonly IEquipmentRepository _repository;
    public UpdateEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto?> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.UpdateAsync(new EquipmentDto(request.Id, request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn, "Available"), cancellationToken);
}

internal sealed class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _repository;
    public DeleteEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllEquipmentsQueryHandler : IRequestHandler<GetAllEquipmentsQuery, IReadOnlyCollection<EquipmentDto>>
{
    private readonly IEquipmentRepository _repository;
    public GetAllEquipmentsQueryHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<IReadOnlyCollection<EquipmentDto>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken) =>
        _repository.GetAllAsync(cancellationToken);
}

internal sealed class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    private readonly IEquipmentRepository _repository;
    public GetEquipmentByIdQueryHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.Id, cancellationToken);
}
