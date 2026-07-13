using Himapp.Safety.Application.Features.Incidents.Commands;
using Himapp.Safety.Application.Features.Incidents.Queries;
using MediatR;

namespace Himapp.Safety.Application.Features.Incidents.Handlers;

internal sealed class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, IncidentDto>
{
    private readonly IIncidentRepository _repository;
    public CreateIncidentCommandHandler(IIncidentRepository repository) => _repository = repository;
    public Task<IncidentDto> Handle(CreateIncidentCommand request, CancellationToken cancellationToken) =>
        _repository.AddAsync(new IncidentDto(0, request.ProjectId, request.Title, request.Severity, request.OccurredOn, request.Description, "Draft", request.Attachment), cancellationToken);
}

internal sealed class UpdateIncidentCommandHandler : IRequestHandler<UpdateIncidentCommand, IncidentDto?>
{
    private readonly IIncidentRepository _repository;
    public UpdateIncidentCommandHandler(IIncidentRepository repository) => _repository = repository;
    public Task<IncidentDto?> Handle(UpdateIncidentCommand request, CancellationToken cancellationToken) =>
        _repository.UpdateAsync(new IncidentDto(request.Id, request.ProjectId, request.Title, request.Severity, request.OccurredOn, request.Description, "Open", request.Attachment), cancellationToken);
}

internal sealed class DeleteIncidentCommandHandler : IRequestHandler<DeleteIncidentCommand, bool>
{
    private readonly IIncidentRepository _repository;
    public DeleteIncidentCommandHandler(IIncidentRepository repository) => _repository = repository;
    public Task<bool> Handle(DeleteIncidentCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllIncidentsQueryHandler : IRequestHandler<GetAllIncidentsQuery, IReadOnlyCollection<IncidentDto>>
{
    private readonly IIncidentRepository _repository;
    public GetAllIncidentsQueryHandler(IIncidentRepository repository) => _repository = repository;
    public Task<IReadOnlyCollection<IncidentDto>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken) =>
        _repository.GetAllAsync(cancellationToken);
}

internal sealed class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDto?>
{
    private readonly IIncidentRepository _repository;
    public GetIncidentByIdQueryHandler(IIncidentRepository repository) => _repository = repository;
    public Task<IncidentDto?> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.Id, cancellationToken);
}
