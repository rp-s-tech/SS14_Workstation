﻿using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Components;

[RegisterComponent]
public sealed partial class NarsiObjectiveComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    public int BloodScore = 20;

    [DataField]
    public bool Completed;
}
