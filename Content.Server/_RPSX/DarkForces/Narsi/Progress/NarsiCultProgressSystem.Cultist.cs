﻿using System.Linq;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress;

public sealed partial class NarsiCultProgressSystem
{
    [Dependency] private readonly SharedEyeSystem _eye = default!;
    private void InitializeCultists()
    {
        SubscribeLocalEvent<NarsiCultistComponent, ComponentInit>(OnCultistInit);
        SubscribeLocalEvent<NarsiCultistLeaderComponent, MobStateChangedEvent>(OnLeaderMobStateChanged);
        SubscribeLocalEvent<NarsiCultistComponent, MobStateChangedEvent>(OnCultistMobStateChanged);
    }

    private void OnCultistInit(EntityUid uid, NarsiCultistComponent component, ComponentInit args)
    {
        if (TryComp<EyeComponent>(uid, out var eye))
            _eye.SetVisibilityMask(uid, eye.VisibilityMask | 220);
        if (_activeProgress != null && _gameTiming.CurTime >= _activeProgress.Value.Comp.ObjectivesSpawnTime)
            AddNarsiObjectives(uid);
    }

    private void OnCultistMobStateChanged(EntityUid uid, NarsiCultistComponent component, MobStateChangedEvent args)
    {
        if (_activeProgress is not {} progress)
            return;

        if (args.NewMobState == MobState.Alive && progress.Comp.LeaderState == LeaderState.NoCandidates)
        {
            SetNewCultistLeader(uid, progress);
        }
    }

    private void OnLeaderMobStateChanged(EntityUid uid, NarsiCultistLeaderComponent component,
        MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        if (_activeProgress == null)
            return;

        var progressComponent = _activeProgress.Value.Comp;
        progressComponent.LeaderState = LeaderState.Dead;

        FindNewCultistLeader();
    }

    public void FindNewCultistLeader()
    {
        if (_activeProgress is not {} progress)
            return;

        if (progress.Comp.LeaderState == LeaderState.Selected)
            return;

        var aliveCultists = EntityQuery<NarsiCultistComponent, MobStateComponent>()
            .Where(cultist => cultist.Item2.CurrentState == MobState.Alive)
            .ToList();

        if (progress.Comp.LeaderEntity != null)
        {
            RemComp<NarsiCultistLeaderComponent>(progress.Comp.LeaderEntity.Value);
        }

        if (!aliveCultists.Any())
        {
            progress.Comp.LeaderEntity = null;
            progress.Comp.LeaderState = LeaderState.NoCandidates;
            return;
        }

        var newLeader = _random.Pick(aliveCultists).Item1.Owner;
        SetNewCultistLeader(newLeader, progress);
    }

    private void SetNewCultistLeader(EntityUid leader, Entity<NarsiCultProgressComponent> progress)
    {
        progress.Comp.LeaderEntity = leader;
        progress.Comp.LeaderState = LeaderState.Selected;

        EnsureComp<NarsiCultistLeaderComponent>(leader);
        SendMessageFromNarsi(progress, "Назначен новый лидер культа!");
    }

    public void SetNewCultistLeader(EntityUid uid)
    {
        if (_activeProgress is not {} progress)
            return;

        SetNewCultistLeader(uid, progress);
    }
}
