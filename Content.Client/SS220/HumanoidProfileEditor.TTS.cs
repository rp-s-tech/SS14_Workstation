using System.Linq;
using Content.Client.SS220.TTS;
using Content.Shared.Preferences;
using Content.Shared.SS220.TTS;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private List<TTSVoicePrototype> _voiceList = new();

    private void InitializeVoice()
    {
        if (_prototypeManager == null)
        {
            Logger.Error("PrototypeManager is null. Voice initialization skipped.");
            return;
        }

        _voiceList = _prototypeManager
            .EnumeratePrototypes<TTSVoicePrototype>()
            .Where(o => o.RoundStart)
            .OrderBy(o => Loc.GetString(o.Name))
            .ToList();

        if (VoiceButton != null)
        {
            VoiceButton.OnItemSelected += args =>
            {
                VoiceButton.SelectId(args.Id);
                SetVoice(_voiceList[args.Id].ID);
            };
        }

        if (VoicePlayButton != null)
        {
            VoicePlayButton.OnPressed += _ => PlayPreviewTTS();
        }
    }

    private void UpdateTTSVoicesControls()
    {
        if (Profile is null)
        {
            Logger.Warning("Profile is null. Cannot update TTS voices controls.");
            return;
        }

        if (VoiceButton == null)
        {
            Logger.Warning("VoiceButton is null. Cannot update TTS voices controls.");
            return;
        }

        VoiceButton.Clear();

        var firstVoiceChoiceId = -1; // Updated default to indicate no valid choice yet.
        for (var i = 0; i < _voiceList.Count; i++)
        {
            var voice = _voiceList[i];
            if (!HumanoidCharacterProfile.CanHaveVoice(voice, Profile.Sex))
                continue;

            var name = Loc.GetString(voice.Name);
            VoiceButton.AddItem(name, i);

            if (firstVoiceChoiceId == -1)
                firstVoiceChoiceId = i;
        }

        var voiceChoiceId = _voiceList.FindIndex(x => x.ID == Profile.Voice);
        if (!VoiceButton.TrySelectId(voiceChoiceId) && firstVoiceChoiceId != -1 &&
            VoiceButton.TrySelectId(firstVoiceChoiceId))
        {
            SetVoice(_voiceList[firstVoiceChoiceId].ID);
        }
    }

    private void PlayPreviewTTS()
    {
        if (Profile is null)
        {
            Logger.Warning("Profile is null. Cannot play TTS preview.");
            return;
        }

        var ttsSystem = _entManager?.System<TTSSystem>();
        if (ttsSystem == null)
        {
            Logger.Error("TTSSystem is null. Cannot play TTS preview.");
            return;
        }

        ttsSystem.RequestPreviewTTS(Profile.Voice);
    }
}
