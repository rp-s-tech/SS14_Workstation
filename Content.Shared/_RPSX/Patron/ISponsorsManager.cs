using System.Diagnostics.CodeAnalysis;
using Content.Shared.Roles;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Patron;

public interface ISponsorsManager
{
    void Initialize();

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public bool TryGetAdditionalSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public bool TryGetSponsorTier([NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public bool TryGetAdditionalSponsorTier([NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public async void AddSponsor(NetUserId userId, SponsorTier tier, int days) { }

    public async void RemoveSponsor(NetUserId userId) { }

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
