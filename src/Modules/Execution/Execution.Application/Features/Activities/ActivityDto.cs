using System.ComponentModel.Design;

namespace Himapp.Execution.Application.Features.Activities;

public sealed record ActivityDto(int Id,int CompanyID, string ActivityName, int UOMID, int CreateBy, int LastModifiedBy);

public sealed record ActivityRequest(int CompanyID, string ActivityName, int UOMID, int CreateBy, int LastModifiedBy);
