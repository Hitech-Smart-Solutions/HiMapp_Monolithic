using Himapp.Admin.Application.Features.Labours.Queries;
using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Handlers;

internal sealed class GetLabourByIdQueryHandler : IRequestHandler<GetLabourByIdQuery, LabourDto?>
{
    private readonly ILabourRepository _repository;

    public GetLabourByIdQueryHandler(ILabourRepository repository) => _repository = repository;

    public async Task<LabourDto?> Handle(GetLabourByIdQuery request, CancellationToken cancellationToken)
    {
        var labour = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return labour?.ToDto();
    }
}
