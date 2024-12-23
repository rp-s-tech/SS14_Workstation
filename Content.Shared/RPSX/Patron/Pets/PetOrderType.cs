using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Patron.Pets;

[Serializable, NetSerializable]
public enum PetOrderType : byte
{
    Stay,
    Follow,
    Attack
}
