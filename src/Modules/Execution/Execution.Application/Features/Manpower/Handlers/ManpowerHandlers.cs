using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class GetAllManpowersQueryHandler : IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>
{
    private readonly IManpowerRepository _repo;
    public GetAllManpowersQueryHandler(IManpowerRepository repo) => _repo = repo;
    public Task<IReadOnlyCollection<ManpowerModel>> Handle(GetAllManpowersQuery request, CancellationToken cancellationToken) => _repo.GetAllAsync(cancellationToken);
}

internal sealed class GetManpowerByIdQueryHandler : IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>
{
    private readonly IManpowerRepository _repo;
    public GetManpowerByIdQueryHandler(IManpowerRepository repo) => _repo = repo;
    public Task<ManpowerModel?> Handle(GetManpowerByIdQuery request, CancellationToken cancellationToken) => _repo.GetByIdAsync(request.Id, cancellationToken);
}

internal sealed class CreateManpowerCommandHandler : IRequestHandler<CreateManpowerCommand, ManpowerModel>
{
    private readonly IManpowerRepository _repo;
    public CreateManpowerCommandHandler(IManpowerRepository repo) => _repo = repo;
    public Task<ManpowerModel> Handle(CreateManpowerCommand request, CancellationToken cancellationToken) => _repo.AddAsync(request.Request, cancellationToken);
}

internal sealed class UpdateManpowerCommandHandler : IRequestHandler<UpdateManpowerCommand, ManpowerModel?>
{
    private readonly IManpowerRepository _repo;
    public UpdateManpowerCommandHandler(IManpowerRepository repo) => _repo = repo;
    public Task<ManpowerModel?> Handle(UpdateManpowerCommand request, CancellationToken cancellationToken) => _repo.UpdateAsync(request.Id, request.Request, cancellationToken);
}

internal sealed class DeleteManpowerCommandHandler : IRequestHandler<DeleteManpowerCommand, bool>
{
    private readonly IManpowerRepository _repo;
    public DeleteManpowerCommandHandler(IManpowerRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id, cancellationToken);
}
