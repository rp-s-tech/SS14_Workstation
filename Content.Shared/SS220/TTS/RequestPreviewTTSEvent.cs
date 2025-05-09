﻿using Robust.Shared.Serialization;

namespace Content.Shared.SS220.TTS;

// ReSharper disable once InconsistentNaming
[Serializable, NetSerializable]
public sealed class RequestPreviewTTSEvent(string voiceId) : EntityEventArgs
{
    public string VoiceId { get; } = voiceId;
}
