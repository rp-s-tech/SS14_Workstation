using System.Text.Json.Nodes;
using Content.Server.GameTicking;

namespace Content.Server.RPSX.Bridges;

public interface IStatusResponseProvider
{
    void GetStatusResponse(JsonNode jObject, GameRunLevel runLevel, DateTime roundStartDateTime, int roundId);
}

public sealed class StubStatusResponseProvider : IStatusResponseProvider
{
    public void GetStatusResponse(JsonNode jObject, GameRunLevel runLevel, DateTime roundStartDateTime, int roundId) { }
}
