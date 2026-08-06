using System.ComponentModel.Design;

namespace Himapp.Execution.Application.Features.Activities.Models;

public sealed record ActivityDto(int Id,int CompanyID, string ActivityName, int UOMID, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate, bool OutputRequired,int CreateBy, int LastModifiedBy);

public sealed record ActivityRequest(int CompanyID, string ActivityName, int UOMID, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate, bool OutputRequired, int CreateBy, int LastModifiedBy);
