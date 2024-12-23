using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Snackbar;
using JetBrains.Annotations;
using System.Numerics;

namespace Content.Client.Snackbar;

[UsedImplicitly]
public sealed class SnackbarEui : BaseEui
{
    private readonly SnackbarWindow _window;

    public SnackbarEui()
    {
        _window = new SnackbarWindow(this);
    }
    public override void HandleMessage(EuiMessageBase msg)
    {
        if (msg is not SnackbarEuiMsg.MessageRequest data)
            return;

        _window.ShowSnackbar(
            duration: data.Duration,
            title: data.Title,
            message: data.Message
        );
    }

    public void SendClosedMessage()
    {
        SendMessage(new SnackbarEuiMsg.Close());
    }

    public override void Closed()
    {
        base.Closed();
        _window.Close();
    }

    public override void Opened()
    {
        _window.OpenCenteredAt(new Vector2(0.5f, 0.08f));
        base.Opened();
    }
}
