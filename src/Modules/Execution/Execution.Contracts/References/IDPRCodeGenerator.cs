using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Contracts.References;

public interface IDPRCodeGenerator
{
    Task<string> GenerateDPRCodeAsync(int projectId, CancellationToken cancellationToken = default);
}
