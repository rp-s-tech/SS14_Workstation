﻿using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;

[Serializable, NetSerializable]
public record NarsiRitualCategoryUIModel(string Name, List<NarsiRitualUIModel> Rituals);
