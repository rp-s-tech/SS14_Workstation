using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Snackbar;


public static class SnackbarEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class Close : EuiMessageBase { }

    [Serializable, NetSerializable]
    public sealed class OnSnackbarEnded : EuiMessageBase{ }

    [Serializable, NetSerializable]
    public sealed class MessageRequest : EuiMessageBase
    {
        public readonly string Title;
        public readonly string Message;


        public readonly int Duration;

        public MessageRequest(int duration, string title, string message)
        {
            Duration = duration;
            Title = title;
            Message = message;
        }
    }
}
