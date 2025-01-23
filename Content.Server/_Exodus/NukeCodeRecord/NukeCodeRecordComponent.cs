namespace Content.Server.Exodus.Nuke.NukeCodeRecord;

[RegisterComponent]
public sealed partial class NukeCodeRecordComponent : Component
{
    [DataField]
    public string? NukeName;

    [DataField]
    public string? NukeCodes;
}
