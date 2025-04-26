﻿using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Progress;

[RegisterComponent]
public sealed partial class RatvarProgressComponent : Component
{
    [DataField]
    public int CurrentPower;

    [DataField]
    public EntityUid RatvarBeaconsObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarConvertObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarPowerObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarSummonObjective = EntityUid.Invalid;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan NextObjectivesCheckTick;

    [DataField]
    public TimeSpan ObjectivesCheckPeriod = TimeSpan.FromSeconds(30);
}
