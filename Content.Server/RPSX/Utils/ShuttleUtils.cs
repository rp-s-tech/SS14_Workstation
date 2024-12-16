using System.Numerics;
using Content.Server.GameTicking;
using Content.Server.Shuttles.Components;
using Content.Server.Station.Components;
using Content.Shared.Station.Components;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Shared.Map;

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
        if (mapId == MapId.Nullspace || !mapSystem.TryLoad(mapId, shuttlePath, out var gridList, GetMapLoadOptions(xOffset, yOffset)))
        {
            return (mapId, shuttleUid);
        }

        shuttleUid = gridList[0];
        entityManager.EnsureComponent<ShuttleComponent>(shuttleUid);
        mapManager.SetMapPaused(mapId, false);

        return (mapId, shuttleUid);
    }

    private static MapLoadOptions? GetMapLoadOptions(int xOffset, int yOffset)
    {
        return new MapLoadOptions()
        {
            Offset = new Vector2(xOffset, yOffset)
        };
    }

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath)
    {
        return CreateShuttleOnExistedMap(mapId, mapManager, mapSystem, entityManager, shuttlePath, 0, 0);
    }

    public static EntityUid CreateShuttleOnExistedMap(MapId mapId, IMapManager mapManager, MapLoaderSystem mapSystem, IEntityManager entityManager, string shuttlePath, int xOffset, int yOffset)
    {
        var shuttleUid = EntityUid.Invalid;
        if (mapId == MapId.Nullspace || !mapSystem.TryLoad(mapId, shuttlePath, out var gridList, GetMapLoadOptions(xOffset, yOffset)) || gridList == null)
        {
            return shuttleUid;
        }

        shuttleUid = gridList[0];
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
