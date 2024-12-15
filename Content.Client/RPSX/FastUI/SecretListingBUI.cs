using System;
using Content.Shared.RPSX.FastUI;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.RPSXClient.FastUI;

public sealed class SecretListingBUI : BoundUserInterface
{

    public SecretListingBUI(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }
    private Client.RPSX.FastUI.SecretListingBUIWindow? _window;
    private NetEntity userEntity = NetEntity.Invalid;

    protected override void Open()
    {
        base.Open();
        _window = new Client.RPSX.FastUI.SecretListingBUIWindow();
        _window.OpenCentered();
        _window.OnClose += Close;
        _window.OnListingButtonPressed += (_, data, key) =>
        {
            SendMessage(new SelectItemMessage(key, data, userEntity));
            _window.Close();
        };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _window?.Dispose();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is SecretListingInitState listingState)
        {
            _window?.UpdateStateByListing(listingState);
            userEntity = listingState.UserEntity;
        }

        if (state is SecretListingInitDataState dataState)
        {
            _window?.UpdateStateByData(dataState);
            userEntity = dataState.UserEntity;
        }
    }
}

