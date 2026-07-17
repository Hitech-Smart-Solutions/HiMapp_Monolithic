using Himapp.Execution.Application.Features.Uom.Models;
using Himapp.Execution.Application.Features.Uom.Commands;
using Himapp.Execution.Application.Features.Uom.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Uom.Handlers;

internal sealed class GetAllUomsQueryHandler : IRequestHandler<GetAllUomsQuery, IReadOnlyCollection<Himapp.Execution.Application.Features.Uom.Models.UomModel>>
{
    private readonly IUomRepository _repo;
    public GetAllUomsQueryHandler(IUomRepository repo) => _repo = repo;
    public Task<IReadOnlyCollection<Himapp.Execution.Application.Features.Uom.Models.UomModel>> Handle(GetAllUomsQuery request, CancellationToken cancellationToken) => _repo.GetAllAsync(cancellationToken);
}

internal sealed class GetUomByIdQueryHandler : IRequestHandler<GetUomByIdQuery, Himapp.Execution.Application.Features.Uom.Models.UomModel?>
{
    private readonly IUomRepository _repo;
    public GetUomByIdQueryHandler(IUomRepository repo) => _repo = repo;
    public Task<Himapp.Execution.Application.Features.Uom.Models.UomModel?> Handle(GetUomByIdQuery request, CancellationToken cancellationToken) => _repo.GetByIdAsync(request.Id, cancellationToken);
}

internal sealed class CreateUomCommandHandler : IRequestHandler<CreateUomCommand, Himapp.Execution.Application.Features.Uom.Models.UomModel>
{
    private readonly IUomRepository _repo;
    public CreateUomCommandHandler(IUomRepository repo) => _repo = repo;
    public Task<Himapp.Execution.Application.Features.Uom.Models.UomModel> Handle(CreateUomCommand request, CancellationToken cancellationToken) => _repo.AddAsync(request.Request, cancellationToken);
}

internal sealed class UpdateUomCommandHandler : IRequestHandler<UpdateUomCommand, Himapp.Execution.Application.Features.Uom.Models.UomModel?>
{
    private readonly IUomRepository _repo;
    public UpdateUomCommandHandler(IUomRepository repo) => _repo = repo;
    public Task<Himapp.Execution.Application.Features.Uom.Models.UomModel?> Handle(UpdateUomCommand request, CancellationToken cancellationToken) => _repo.UpdateAsync(request.Id, request.Request, cancellationToken);
}

internal sealed class DeleteUomCommandHandler : IRequestHandler<DeleteUomCommand, bool>
{
    private readonly IUomRepository _repo;
    public DeleteUomCommandHandler(IUomRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteUomCommand request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id, cancellationToken);
}
