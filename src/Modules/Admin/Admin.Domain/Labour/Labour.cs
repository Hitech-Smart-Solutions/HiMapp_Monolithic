using Himapp.Admin.Domain.Labour.Events;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.Admin.Domain.Labour;

public sealed class Labour : BaseEntity
{
    public long ProjectId { get; private set; }
    public long ContractorId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string AadhaarMasked { get; private set; } = string.Empty;
    public byte[] AadhaarHash { get; private set; } = [];
    public string? Pan { get; private set; }
    public long PhotoFileId { get; private set; }
    public LabourStatus Status { get; private set; } = LabourStatus.Registered;

    private Labour()
    {
    }

    public static Labour Register(long projectId, long contractorId, string name, DateOnly dateOfBirth, string aadhaarMasked, byte[] aadhaarHash, string? pan, long photoFileId)
    {
        var labour = new Labour
        {
            ProjectId = projectId,
            ContractorId = contractorId,
            Name = name,
            DateOfBirth = dateOfBirth,
            AadhaarMasked = aadhaarMasked,
            AadhaarHash = aadhaarHash,
            Pan = pan,
            PhotoFileId = photoFileId,
            Status = LabourStatus.InductionPending
        };

        labour.Raise(new LabourRegistered(projectId, labour.Id, name, contractorId));
        return labour;
    }

    public void UpdateProfile(long projectId, long contractorId, string name, DateOnly dateOfBirth, string aadhaarMasked, byte[] aadhaarHash, string? pan, long photoFileId)
    {
        ProjectId = projectId;
        ContractorId = contractorId;
        Name = name;
        DateOfBirth = dateOfBirth;
        AadhaarMasked = aadhaarMasked;
        AadhaarHash = aadhaarHash;
        Pan = pan;
        PhotoFileId = photoFileId;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void ApplyClearance(bool inductionOk, bool testsOk, bool medicalOk)
    {
        Status = inductionOk && testsOk && medicalOk ? LabourStatus.Cleared : LabourStatus.InductionPending;
    }
}

public enum LabourStatus
{
    Registered,
    InductionPending,
    TestsPending,
    Cleared,
    GatepassIssued
}
