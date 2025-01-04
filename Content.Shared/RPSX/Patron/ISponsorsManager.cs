using System.Diagnostics.CodeAnalysis;
using Content.Shared.Roles;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Network;

namespace Content.Shared.RPSX.Patron;

public interface ISponsorsManager
{
    void Initialize();

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public bool TryGetSponsorTier([NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = null;
        return false;
    }

    public async void AddSponsor(NetUserId userId, string tier) { }

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
