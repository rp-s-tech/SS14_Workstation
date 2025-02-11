using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Random;


namespace Content.Server.RPSX.RandomTeleport;

public sealed class RandomTeleportSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;

    private const int MaxRandomTeleportAttempts = 20;

    public EntityCoordinates? GetRandomCoordinates(EntityUid uid, float radius)
    {
        var xform = Transform(uid);
        var coords = xform.Coordinates;
        var newCoords = coords.Offset(_random.NextVector2(radius));

        for (var i = 0; i < MaxRandomTeleportAttempts; i++)
        {
            var randVector = _random.NextVector2(radius);
            newCoords = coords.Offset(randVector);

            if (!_entityLookup.GetEntitiesIntersecting(_xform.ToMapCoordinates(newCoords), LookupFlags.Static).Any())
                break;
        }
        return newCoords;
    }
}
