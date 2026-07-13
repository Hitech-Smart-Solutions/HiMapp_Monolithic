using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Himapp.Admin.Application;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminModule(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/admin").WithTags("Admin");

        group.MapGet("/module", () => Results.Ok(new
        {
            Module = "Admin",
            Capabilities = new[] { "Labour registration", "Labour gatepass issue/renew", "Project and contractor directory" },
            Publishes = new[] { "Admin.LabourRegistered" },
            Contracts = new[] { "ILabourLookup", "IContractorLookup", "IWorkCategoryLookup", "IProjectDirectory" }
        }));

        return endpoints;
    }
}
