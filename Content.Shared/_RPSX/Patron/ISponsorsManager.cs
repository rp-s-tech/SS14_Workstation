using System.Diagnostics.CodeAnalysis;
using Content.Shared.Roles;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Patron;

public interface ISponsorsManager
{
    void Initialize();

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out AllSponsorInfo? sponsor)
    {
        sponsor = null;
        return false;
    }

    public bool TryGetSponsorTier([NotNullWhen(true)] out AllSponsorInfo? sponsor)
    {
        sponsor = null;
        return false;
    }

    public void AddSponsor(NetUserId userId, SponsorTier tier, int days) { }

    public void RemoveSponsor(NetUserId userId, SponsorTier tier) { }

    public bool IsUserHasRoleTimeByPass(NetUserId userId)
    {
        return false;
    }

    public bool IsJobAvailable(JobPrototype job)
    {
        return false;
    }

    public bool IsJobAvailable(NetUserId userId, JobPrototype job)
    {
        return false;
    }
}

[Serializable, NetSerializable]
public sealed class UpdateAdditionalSponsorshipData : EventArgs
{
    public NetUserId UserId { get; }
    public string Tier { get; } = "";

    public UpdateAdditionalSponsorshipData(NetUserId userId, string tier)
    {
        Tier = tier;
        UserId = userId;
    }
}
