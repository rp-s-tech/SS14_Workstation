using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.Verbs;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem
{
    [Dependency] private readonly ShuttleSystem _shuttle = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private void InitShuttle()
    {
        SubscribeLocalEvent<PirateShuttleConsoleComponent, GetVerbsEvent<Verb>>(AddEscapeVerb);
        SubscribeLocalEvent<PirateShuttleComponent, FTLCompletedEvent>(OnEscaped);
    }

    private void AddEscapeVerb(EntityUid uid, PirateShuttleConsoleComponent component, GetVerbsEvent<Verb> args)
    {
        if (args.Hands == null || !args.CanAccess || !args.CanInteract)
            return;

        if (!HasComp<PirateComponent>(args.User))
            return;

        if (!TryComp<PiratesProgressComponent>(component.PiratesProgress, out var progressComponent))
            return;

        if (Transform(progressComponent.TargetStation).MapUid != Transform(uid).MapUid)
            return;

        Verb verb = new()
        {
            Text = Loc.GetString("pirates-escape"),
            Act = () => Escape(uid, (component.PiratesProgress, progressComponent))
        };
        args.Verbs.Add(verb);
    }

    private void Escape(Entity<ShuttleComponent?> shuttle, Entity<PiratesProgressComponent> progress)
    {
        if (!Resolve(shuttle, ref shuttle.Comp)) return;

        if (!progress.Comp.PiratesStartMap.HasValue)
            return;

        _shuttle.FTLToCoordinates(shuttle, shuttle.Comp,
                        new EntityCoordinates(progress.Comp.PiratesStartMap.Value,
                            _random.NextVector2(1000f)), Angle.Zero);

        progress.Comp.RoundCanBeEnded = true;
    }

    private void OnEscaped(Entity<PirateShuttleComponent> entity, ref FTLCompletedEvent args)
    {
        if (!TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent))
            return;

        if (args.MapUid != progressComponent.PiratesStartMap)
            return;

        CheckRoundShouldEnd((entity.Comp.PiratesProgress, progressComponent));
    }
}