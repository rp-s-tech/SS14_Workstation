using Content.Client.Eui;
using Content.Shared.Eui;
using JetBrains.Annotations;
using static Content.Shared.FastUI.SecretListingEUIState;

namespace Content.Client.RPSX.FastUI;

[UsedImplicitly]
public sealed class SecretListingEUI : BaseEui
{
    private NetEntity netEntity = NetEntity.Invalid;
    private readonly SecretListingBUIWindow _window;

    public SecretListingEUI()
    {
        _window = new SecretListingBUIWindow();
        _window.OnClose += OnClosed;
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        if (msg is not SecretListingEUIInitState data)
            return;

        netEntity = data.NetEntity;
        _window.UpdateStateByDataEUI(data);
        base.HandleMessage(msg);
    }

    private void OnClosed()
    {
        SendMessage(new CloseEuiMessage());
    }

    public override void Closed()
    {
        base.Closed();
        _window.Close();
    }

    public override void Opened()
    {
        base.Opened();

        _window.OpenCentered();
        _window.OnListingButtonPressed += (_, data, key) =>
        {
            SendMessage(new SelectItemEUIMessage(key, data, netEntity));
            _window.Close();
        };
    }

}
