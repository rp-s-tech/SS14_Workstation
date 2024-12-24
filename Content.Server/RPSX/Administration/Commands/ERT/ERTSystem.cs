using Content.Server.Chat.Systems;
using Content.Server.RPSX.FastUI;
using Content.Server.RPSX.Utils;
using Content.Shared.GameTicking;
using Content.Shared.RPSX.FastUI;
using JetBrains.Annotations;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Administration.Commands.ERT;

[UsedImplicitly]
public sealed class ERTSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoaderSystem = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    private ERTStatus ERTStatus = ERTStatus.IDLE;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
        SubscribeLocalEvent<SecretListingEUISelectedEvent>(OnSelectedItemMessage);
    }

    public void SpawnERT(string id)
    {
        var ertPrototype = _prototypeManager.Index<ERTGroupPrototype>(id);

        SpawnShuttle(ertPrototype.Path.ToString());
        SendMessage(ertPrototype.MessageOnSpawn);

        if (ertPrototype.SoundOnSpawn != null)
        {
            _audio.PlayGlobal(ertPrototype.SoundOnSpawn, Filter.Broadcast(), true, AudioParams.Default.WithVolume(-2f));
        }

        ERTStatus = ERTStatus.CALLED;
    }
    private void OnSelectedItemMessage(SecretListingEUISelectedEvent args)
    {
        if (args.Key != "ERTGroupsListing")
            return;

        SpawnERT(args.Data.ID);
    }

    private void OnRoundEnded(RoundEndedEvent ev)
    {
        ERTStatus = ERTStatus.IDLE;
    }

    public void CallERT(ICommonSession playerSession)
    {
        if (ERTStatus != ERTStatus.IDLE)
            return;

        var prototype = _prototypeManager.Index<SecretListingCategoryPrototype>("ERTGroupsListing");
        SecretListingEUI.ShowSecretListingEUI(EntityManager, playerSession, prototype, true);
    }

    public void DropStatus()
    {
        ERTStatus = ERTStatus.IDLE;
    }

    private void SendMessage(string locCode)
    {
        ChatUtils.SendMessageFromCentcom(
            chatSystem: _chatSystem,
            message: locCode,
            sender: "Центральное командование",
            stationId: null
        );
    }

    private void SpawnShuttle(string shuttlePath)
    {
        ShuttleUtils.CreateShuttleOnNewMap(
            mapManager: _mapManager,
            mapSystem: _mapLoaderSystem,
            entityManager: EntityManager,
            shuttlePath: shuttlePath
        );
    }
}
