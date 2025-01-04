using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Eye.NightVision.Events;

[Serializable]
[NetSerializable]
public sealed class NightVisionChangedEvent : EntityEventArgs;

