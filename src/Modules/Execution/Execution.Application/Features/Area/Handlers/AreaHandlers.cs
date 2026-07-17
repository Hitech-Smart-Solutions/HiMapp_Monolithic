using Himapp.Execution.Application.Features.Area.Models;
using Himapp.Execution.Application.Features.Area.Commands;
using Himapp.Execution.Application.Features.Area.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Area.Handlers;

internal sealed class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, IReadOnlyCollection<AreaModel>>
{
    private readonly IAreaRepository _repo;
    public GetAllAreasQueryHandler(IAreaRepository repo) => _repo = repo;
    public Task<IReadOnlyCollection<AreaModel>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken) => _repo.GetAllAsync(cancellationToken);
}

internal sealed class GetAreaByIdQueryHandler : IRequestHandler<GetAreaByIdQuery, AreaModel?>
{
    private readonly IAreaRepository _repo;
    public GetAreaByIdQueryHandler(IAreaRepository repo) => _repo = repo;
    public Task<AreaModel?> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken) => _repo.GetByIdAsync(request.Id, cancellationToken);
}

internal sealed class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, AreaModel>
{
    private readonly IAreaRepository _repo;
    public CreateAreaCommandHandler(IAreaRepository repo) => _repo = repo;
    public Task<AreaModel> Handle(CreateAreaCommand request, CancellationToken cancellationToken) => _repo.AddAsync(request.Request, cancellationToken);
}

internal sealed class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, AreaModel?>
{
    private readonly IAreaRepository _repo;
    public UpdateAreaCommandHandler(IAreaRepository repo) => _repo = repo;
    public Task<AreaModel?> Handle(UpdateAreaCommand request, CancellationToken cancellationToken) => _repo.UpdateAsync(request.Id, request.Request, cancellationToken);
}

internal sealed class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, bool>
{
    private readonly IAreaRepository _repo;
    public DeleteAreaCommandHandler(IAreaRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteAreaCommand request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id, cancellationToken);
}
