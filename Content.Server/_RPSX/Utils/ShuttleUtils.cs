using System.Numerics;
using Content.Server.GameTicking;
using Content.Server.Shuttles.Components;
using Content.Server.Station.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Map;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Utility;
using System.Linq;
using Robust.Shared.Map.Components;

namespace Content.Server.RPSX.Utils;

public static class ShuttleUtils
{
    public static (MapId, EntityUid) CreateShuttleOnNewMap(SharedMapSystem mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath)
    {
        return CreateShuttleOnNewMap(mapManager, mapSystem, entityManager, shuttlePath, 0, 0);
    }

    public static (MapId, EntityUid) CreateShuttleOnNewMap(SharedMapSystem mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath, int xOffset = 0, int yOffset = 0)
    {
        var shuttleUid = EntityUid.Invalid;
        mapManager.CreateMap(out var mapId);

        var resPath = new ResPath(shuttlePath);
        var options = GetMapLoadOptions(xOffset, yOffset);

        if (!mapSystem.TryLoadGrid(mapId, resPath, out var grid))
        {
            return (mapId, shuttleUid);
        }
        shuttleUid = grid.Value;

        entityManager.EnsureComponent<ShuttleComponent>(shuttleUid);
        mapManager.SetPaused(mapId, false);

        return (mapId, shuttleUid);
    }

    private static MapLoadOptions GetMapLoadOptions(int xOffset, int yOffset)
    {
        return new MapLoadOptions
        {
            Offset = new Vector2(xOffset, yOffset),
            DeserializationOptions = new DeserializationOptions()
        };
    }

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, SharedMapSystem mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath)
    {
        return CreateShuttleOnExistedMap(mapId, mapManager, mapSystem, entityManager, shuttlePath, 0, 0);
    }

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, SharedMapSystem mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath, int xOffset, int yOffset)
    {
        var shuttleUid = EntityUid.Invalid;
        var resPath = new ResPath(shuttlePath);
        var options = GetMapLoadOptions(xOffset, yOffset);

        if (mapId == MapId.Nullspace || !mapSystem.TryMergeMap(mapId, resPath, out var gridList, options.DeserializationOptions) || gridList == null)
        {
            return shuttleUid;
        }

        if (gridList.Count > 0)
        {
            shuttleUid = gridList.First().Owner;
        }

        entityManager.EnsureComponent<ShuttleComponent>(shuttleUid);
        mapManager.SetPaused(mapId, false);

        return shuttleUid;
    }

    public static EntityUid GetTargetStation(GameTicker ticker, SharedMapSystem mapManager, IEntityManager entityManager)
    {
        var targetmap = ticker.DefaultMap;
        var targetStation = EntityUid.Invalid;

        foreach (var grid in GetAllMapGrids(targetmap, entityManager))
        {
            if (!entityManager.TryGetComponent<StationMemberComponent>(grid.Owner, out var stationMember))
                continue;

            if (!entityManager.TryGetComponent<StationDataComponent>(stationMember.Station, out _))
                continue;

            targetStation = stationMember.Station;
            break;
        }

        return targetStation;
    }

    public static IEnumerable<MapGridComponent> GetAllMapGrids(MapId mapId, IEntityManager entityManager)
    {
        var query = entityManager.AllEntityQueryEnumerator<MapGridComponent, TransformComponent>();
        while (query.MoveNext(out var grid, out var xform))
        {
            if (xform.MapID == mapId)
                yield return grid;
        }
    }

}
