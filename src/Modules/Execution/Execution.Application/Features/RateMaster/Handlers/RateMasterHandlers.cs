using Himapp.Execution.Application.Features.RateMaster.Models;
using Himapp.Execution.Application.Features.RateMaster.Commands;
using Himapp.Execution.Application.Features.RateMaster.Queries;
using MediatR;

namespace Himapp.Execution.Application.Features.RateMaster.Handlers;

internal sealed class CreateRateMasterCommandHandler : IRequestHandler<CreateRateMasterCommand, RateMasterModel>
{
    private readonly Features.RateMaster.IRateMasterRepository _repo;
    public CreateRateMasterCommandHandler(Features.RateMaster.IRateMasterRepository repo) => _repo = repo;
    public Task<RateMasterModel> Handle(CreateRateMasterCommand request, CancellationToken cancellationToken) => _repo.AddAsync(request.Request, cancellationToken);
}

internal sealed class UpdateRateMasterCommandHandler : IRequestHandler<UpdateRateMasterCommand, RateMasterModel?>
{
    private readonly Features.RateMaster.IRateMasterRepository _repo;
    public UpdateRateMasterCommandHandler(Features.RateMaster.IRateMasterRepository repo) => _repo = repo;
    public Task<RateMasterModel?> Handle(UpdateRateMasterCommand request, CancellationToken cancellationToken) => _repo.UpdateAsync(request.Id, request.Request, cancellationToken);
}

internal sealed class DeleteRateMasterCommandHandler : IRequestHandler<DeleteRateMasterCommand, bool>
{
    private readonly Features.RateMaster.IRateMasterRepository _repo;
    public DeleteRateMasterCommandHandler(Features.RateMaster.IRateMasterRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteRateMasterCommand request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllRateMastersQueryHandler : IRequestHandler<GetAllRateMastersQuery, IReadOnlyCollection<RateMasterModel>>
{
    private readonly Features.RateMaster.IRateMasterRepository _repo;
    public GetAllRateMastersQueryHandler(Features.RateMaster.IRateMasterRepository repo) => _repo = repo;
    public Task<IReadOnlyCollection<RateMasterModel>> Handle(GetAllRateMastersQuery request, CancellationToken cancellationToken) => _repo.GetAllAsync(cancellationToken);
}

internal sealed class GetRateMasterByIdQueryHandler : IRequestHandler<GetRateMasterByIdQuery, RateMasterModel?>
{
    private readonly Features.RateMaster.IRateMasterRepository _repo;
    public GetRateMasterByIdQueryHandler(Features.RateMaster.IRateMasterRepository repo) => _repo = repo;
    public Task<RateMasterModel?> Handle(GetRateMasterByIdQuery request, CancellationToken cancellationToken) => _repo.GetByIdAsync(request.Id, cancellationToken);
}
