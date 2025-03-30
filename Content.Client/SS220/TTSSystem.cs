// Refactored TTS from SS220 to RPSX
// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.CCVar;
using Content.Shared.SS220.TTS;
using Content.Shared.SS220.TTS.Commands;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.SS220.TTS;

public sealed partial class TTSSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IDependencyCollection _dep = default!;

    private ISawmill _sawmill = default!;

    private static readonly MemoryContentRoot ContentRoot = new();
    private static readonly ResPath Prefix = ResPath.Root / "TTS";
    private static readonly AudioResource EmptyAudioResource = new();
    private static bool _rootInitialized = false;

    private float _volume = 1f;
    private float _radioVolume = 1f;
    private int _fileIdx = 0;

    private const int MaxQueuedPerEntity = 20;
    private const int MaxEntitiesQueued = 30;
    private readonly Dictionary<EntityUid, Queue<PlayRequest>> _playQueues = new();
    private readonly Dictionary<EntityUid, EntityUid?> _playingStreams = new();

    private EntityUid _fallbackEntity = EntityUid.Invalid;

    private const float TtsMultiplier = 4f;
    private const float TtsRadioMultiplier = 4f;

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("tts");

        if (!_rootInitialized)
        {
            _resourceCache.AddRoot(Prefix, ContentRoot);
            _rootInitialized = true;
        }

        _cfg.OnValueChanged(TTSVars.TTSVolume, v => _volume = v, true);
        _cfg.OnValueChanged(TTSVars.TTSRadioVolume, v => _radioVolume = v, true);

        SubscribeNetworkEvent<PlayTTSEvent>(OnPlayTTS);
        SubscribeNetworkEvent<TtsQueueResetMessage>(OnQueueResetRequest);

        InitializeAnnounces();
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _cfg.UnsubValueChanged(TTSVars.TTSVolume, v => _volume = v);
        _cfg.UnsubValueChanged(TTSVars.TTSRadioVolume, v => _radioVolume = v);

        // clear virtual files
        ContentRoot.Clear();

        ShutdownAnnounces();
        ResetQueuesAndEndStreams();
    }

    public void RequestPreviewTTS(string voiceId)
    {
        RaiseNetworkEvent(new RequestPreviewTTSEvent(voiceId));
    }

    private void OnQueueResetRequest(TtsQueueResetMessage _)
    {
        ResetQueuesAndEndStreams();
        _sawmill.Debug("TTS queue reset by server request.");
    }

    public void ResetQueuesAndEndStreams()
    {
        foreach (var (uid, stream) in _playingStreams)
        {
            _playingStreams[uid] = _audio.Stop(stream);
        }

        _playingStreams.Clear();
        _playQueues.Clear();
        ContentRoot.Clear();
    }

    public override void FrameUpdate(float frameTime)
    {
        var finished = new List<EntityUid>();

        foreach (var (uid, ent) in _playingStreams)
        {
            if (!EntityManager.TryGetComponent<AudioComponent>(ent, out _))
                finished.Add(uid);
        }

        foreach (var uid in finished)
            _playingStreams.Remove(uid);

        var removeQueue = new List<EntityUid>();

        foreach (var (uid, queue) in _playQueues)
        {
            if (_playingStreams.ContainsKey(uid))
                continue;

            if (!queue.TryDequeue(out var req))
                continue;

            if (queue.Count == 0)
                removeQueue.Add(uid);

            ResPath? localFile = null;
            SoundPathSpecifier spec;

            switch (req)
            {
                case PlayRequestById rid:
                    localFile = new ResPath($"{rid.FileIdx}.wav");
                    spec = new SoundPathSpecifier(Prefix / localFile.Value, rid.Params);
                    break;

                case PlayRequestByPath rpath:
                    spec = new SoundPathSpecifier(rpath.Path, rpath.Params);
                    break;

                default:
                    continue;
            }

            (EntityUid entity, AudioComponent comp)? stream;
            if (req.PlayGlobal)
            {
                stream = _audio.PlayGlobal(spec, Filter.Local(), false);
            }
            else
            {
                stream = _audio.PlayEntity(spec, _fallbackEntity, uid);
            }

            if (stream is { comp: not null })
                _playingStreams[uid] = stream.Value.entity;

            if (localFile != null)
                RemoveFile(Prefix / localFile.Value);
        }

        foreach (var uid in removeQueue)
            _playQueues.Remove(uid);
    }

    private void RemoveFile(ResPath path)
    {
        ContentRoot.RemoveFile(path);
        _resourceCache.CacheResource(path, EmptyAudioResource);
    }

    private void PlayTTSBytes(byte[] data, EntityUid? sourceUid, AudioParams? audioParams, bool globally = false)
    {
        if (data.Length == 0)
            return;

        var param = audioParams ?? AudioParams.Default;

        var filePath = new ResPath($"{_fileIdx}.wav");
        ContentRoot.AddOrUpdateFile(filePath, data);

        var res = new AudioResource();
        res.Load(_dep, Prefix / filePath);
        _resourceCache.CacheResource(Prefix / filePath, res);

        if (sourceUid == null)
        {
            var spec = new SoundPathSpecifier(Prefix / filePath, param);
            _audio.PlayGlobal(spec, Filter.Local(), false);
            RemoveFile(Prefix / filePath);
        }
        else if (sourceUid.Value.IsValid())
        {
            TryQueuePlayById(sourceUid.Value, _fileIdx, param, globally);
        }

        _fileIdx++;
    }

    private void OnPlayTTS(PlayTTSEvent ev)
    {
        var sourceEntity = GetEntity(ev.SourceUid);

        if (ev.IsRadio && sourceEntity == _fallbackEntity)
        {
            return;
        }

        var gain = ev.IsRadio ? _radioVolume : _volume;
        var multiplier = ev.IsRadio ? TtsRadioMultiplier : TtsMultiplier;

        var finalGain = Math.Clamp(gain * multiplier, 0f, 1f);

        if (finalGain <= 0.001f)
        {
            return;
        }

        var volume = SharedAudioSystem.GainToVolume(finalGain);
        if (volume < -32f)
            volume = -32f;

        _sawmill.Debug($"[TTS] PlayTTS: radio={ev.IsRadio}, gain={gain}, mult={multiplier}, clampedGain={finalGain}, finalVol={volume}");

        var param = AudioParams.Default.WithVolume(volume);
        PlayTTSBytes(ev.Data, sourceEntity, param, ev.IsRadio);
    }


    public void TryQueueRequest(EntityUid uid, PlayRequest req)
    {
        if (!_playQueues.TryGetValue(uid, out var queue))
        {
            if (_playQueues.Count >= MaxEntitiesQueued)
                return;
            queue = new();
            _playQueues[uid] = queue;
        }

        if (queue.Count < MaxQueuedPerEntity)
            queue.Enqueue(req);
    }

    public void TryQueuePlayById(EntityUid uid, int fileIdx, AudioParams param, bool globally = false)
    {
        TryQueueRequest(uid, new PlayRequestById(fileIdx, param, globally));
    }

    public void PlaySoundQueued(EntityUid uid, ResPath path, AudioParams? prms = null, bool globally = false)
    {
        TryQueueRequest(uid, new PlayRequestByPath(path, prms, globally));
    }

    // Requests
    public abstract class PlayRequest
    {
        public readonly AudioParams Params;
        public readonly bool PlayGlobal;

        protected PlayRequest(AudioParams? audioParams = null, bool global = false)
        {
            Params = audioParams ?? AudioParams.Default;
            PlayGlobal = global;
        }
    }

    public sealed class PlayRequestByPath : PlayRequest
    {
        public readonly ResPath Path;
        public PlayRequestByPath(ResPath p, AudioParams? prms = null, bool g = false) : base(prms, g) => Path = p;
    }

    public sealed class PlayRequestById : PlayRequest
    {
        public readonly int FileIdx;
        public PlayRequestById(int idx, AudioParams? prms = null, bool g = false) : base(prms, g) => FileIdx = idx;
    }
}
