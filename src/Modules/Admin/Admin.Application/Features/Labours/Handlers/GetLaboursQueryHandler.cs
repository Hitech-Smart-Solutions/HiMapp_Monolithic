using Himapp.Admin.Application.Features.Labours.Queries;
using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Handlers;

internal sealed class GetLaboursQueryHandler : IRequestHandler<GetLaboursQuery, IReadOnlyCollection<LabourDto>>
{
    private readonly ILabourRepository _repository;

    public GetLaboursQueryHandler(ILabourRepository repository) => _repository = repository;

    public async Task<IReadOnlyCollection<LabourDto>> Handle(GetLaboursQuery request, CancellationToken cancellationToken)
    {
        var labours = await _repository.GetAllAsync(cancellationToken);
        return labours.Select(labour => labour.ToDto()).ToArray();
    }
}
