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

namespace Content.Server.RPSX.Utils;

public static class ShuttleUtils
{
    public static (MapId, EntityUid) CreateShuttleOnNewMap(IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath)
    {
        return CreateShuttleOnNewMap(mapManager, mapSystem, entityManager, shuttlePath, 0, 0);
    }

    public static (MapId, EntityUid) CreateShuttleOnNewMap(IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath, int xOffset = 0, int yOffset = 0)
    {
        var shuttleUid = EntityUid.Invalid;
        var mapId = mapManager.CreateMap();

        var resPath = new ResPath(shuttlePath);
        var options = GetMapLoadOptions(xOffset, yOffset);

        if (mapId == MapId.Nullspace || !mapSystem.TryLoadMapWithId(mapId, resPath, out var mapEntity, out var gridList, options.DeserializationOptions))
        {
            return (mapId, shuttleUid);
        }

        if (gridList.Count > 0)
        {
            shuttleUid = gridList.First().Owner;
        }

        entityManager.EnsureComponent<ShuttleComponent>(shuttleUid);
        mapManager.SetMapPaused(mapId, false);

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

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath)
    {
        return CreateShuttleOnExistedMap(mapId, mapManager, mapSystem, entityManager, shuttlePath, 0, 0);
    }

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath, int xOffset, int yOffset)
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
        mapManager.SetMapPaused(mapId, false);

        return shuttleUid;
    }

    public static EntityUid GetTargetStation(GameTicker ticker, IMapManager mapManager, IEntityManager entityManager)
    {
        var targetmap = ticker.DefaultMap;
        var targetStation = EntityUid.Invalid;

        foreach (var grid in mapManager.GetAllMapGrids(targetmap))
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
}
