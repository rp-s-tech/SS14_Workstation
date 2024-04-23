using Content.Server.Nuke;
using Content.Server.Station.Systems;
using Content.Shared.Examine;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.Exodus.Nuke.NukeCodeRecord;

public sealed class NukeCodeRecordSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StationSystem _station = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NukeCodeRecordComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<NukeCodeRecordComponent, ExaminedEvent>(OnExamine);
    }

    private void OnInit(EntityUid uid, NukeCodeRecordComponent component, ComponentInit args)
    {
        TrySetRelativeNukeCode(uid, component);
    }

    private void OnExamine(EntityUid uid, NukeCodeRecordComponent component, ExaminedEvent args)
    {
        if (component.NukeName != null && component.NukeCodes != null)
            args.PushMarkup(Loc.GetString("nuke-codes-record-examine-filled", ("nukeName", component.NukeName), ("nukeCode", component.NukeCodes)));
        else
            args.PushText(Loc.GetString("nuke-codes-record-examine-empty"));
    }

    private bool TrySetRelativeNukeCode(
            EntityUid uid,
            NukeCodeRecordComponent component,
            EntityUid? station = null,
            TransformComponent? transform = null)
    {
        if (!Resolve(uid, ref transform))
        {
            return false;
        }

        bool nukeFound = false;
        var owningStation = station ?? _station.GetOwningStation(uid);
        var nukes = new List<Entity<NukeComponent>>();
        var query = EntityQueryEnumerator<NukeComponent>();

        while (query.MoveNext(out var nukeUid, out var nuke))
        {
            nukes.Add((nukeUid, nuke));
        }

        _random.Shuffle(nukes);

        foreach (var (nukeUid, nuke) in nukes)
        {
            if (nuke.OriginStation != owningStation)
            {
                continue;
            }

            nukeFound = true;
            component.NukeName = MetaData(nukeUid).EntityName;
            component.NukeCodes = nuke.Code;
            break;
        }

        return nukeFound;
    }
}
