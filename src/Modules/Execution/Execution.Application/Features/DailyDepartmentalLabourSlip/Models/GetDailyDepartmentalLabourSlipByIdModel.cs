using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models
{
    public sealed class GetDailyDepartmentalLabourSlipByIdModel
    {
        public int Id { get; init; }
        public Guid UniqueId { get; init; }

        public int? ProjectId { get; init; }
        public string? ProjectName { get; init; }

        public DateTime? SlipDate { get; init; }
        public string? DDLSlipCode { get; init; }
        public string? IssueNumber { get; init; }

        public int? PartyID { get; init; }
        public string? ContractorName { get; init; }

        public string? Remarks { get; init; }

        public int StatusID { get; init; }
        public bool IsActive { get; init; }

        public int CreatedBy { get; init; }
        public string? CreatedName { get; init; }

        public DateTime CreatedDate { get; init; }

        public int LastModifiedBy { get; init; }
        public DateTime LastModifiedDate { get; init; }

        public int? IsAwaitingApprovalForId { get; init; }

        public IReadOnlyCollection<GetDailyDepartmentalLabourSlipDetailsModel> Details { get; init; }

        public GetDailyDepartmentalLabourSlipByIdModel(
            int id,
            Guid uniqueId,
            int? projectId,
            string? projectName,
            DateTime? slipDate,
            string? ddlSlipCode,
            string? issueNumber,
            int? partyId,
            string? contractorName,
            string? remarks,
            int statusId,
            bool isActive,
            int createdBy,
            string? createdName,
            DateTime createdDate,
            int lastModifiedBy,
            DateTime lastModifiedDate,
            IReadOnlyCollection<GetDailyDepartmentalLabourSlipDetailsModel> details,
            int? isAwaitingApprovalForId = null)
        {
            Id = id;
            UniqueId = uniqueId;
            ProjectId = projectId;
            ProjectName = projectName;
            SlipDate = slipDate;
            DDLSlipCode = ddlSlipCode;
            IssueNumber = issueNumber;
            PartyID = partyId;
            ContractorName = contractorName;
            Remarks = remarks;
            StatusID = statusId;
            IsActive = isActive;
            CreatedBy = createdBy;
            CreatedName = createdName;
            CreatedDate = createdDate;
            LastModifiedBy = lastModifiedBy;
            LastModifiedDate = lastModifiedDate;
            Details = details ?? Array.Empty<GetDailyDepartmentalLabourSlipDetailsModel>();
            IsAwaitingApprovalForId = isAwaitingApprovalForId;
        }
    }

    public sealed class GetDailyDepartmentalLabourSlipDetailsModel
    {
        public int Id { get; init; }
        public Guid UniqueId { get; init; }
        public int? LabourCategoryTypeId { get; init; }
        public string? LabourCategoryTypeName { get; init; }
        public bool? IsLumSumWork { get; init; }
        public int? NumOfLabour { get; init; }
        public DateTime? FromTime { get; init; }
        public DateTime? ToTime { get; init; }
        public decimal? LunchHour { get; init; }
        public decimal? WorkingHours { get; init; }
        public int? WorkLocationId { get; init; }
        public int? ActivityId { get; init; }
        public string? ActivityName { get; init; }
        public string? ActivityDetails { get; init; }
        public int? UomId { get; init; }
        public string? UOMShortName { get; set; }
        public decimal? Quantity { get; init; }

        public int? DebitPartyId { get; init; }
        public string? DebitPartyName { get; init; }

        public string? Remarks { get; init; }

        public GetDailyDepartmentalLabourSlipDetailsModel(
            int id,
            Guid uniqueId,
            int? labourCategoryTypeId,
            string? labourCategoryTypeName,
            bool? isLumSumWork,
            int? numOfLabour,
            DateTime? fromTime,
            DateTime? toTime,
            decimal? lunchHour,
            decimal? workingHours,
            int? workLocationId,
            int? activityId,
            string? activityName,
            string? activityDetails,
            int? uomId,
            string? uomShortName,
            decimal? quantity,
            int? debitPartyId,
            string? debitPartyName,
            string? remarks)
        {
            Id = id;
            UniqueId = uniqueId;
            LabourCategoryTypeId = labourCategoryTypeId;
            LabourCategoryTypeName  = labourCategoryTypeName;
            IsLumSumWork = isLumSumWork;
            NumOfLabour = numOfLabour;
            FromTime = fromTime;
            ToTime = toTime;
            LunchHour = lunchHour;
            WorkingHours = workingHours;
            WorkLocationId = workLocationId;
            ActivityId = activityId;
            ActivityName = activityName;
            ActivityDetails = activityDetails;
            UomId = uomId;
            UOMShortName = uomShortName;
            Quantity = quantity;
            DebitPartyId = debitPartyId;
            DebitPartyName = debitPartyName;
            Remarks = remarks;
        }
    }
}
