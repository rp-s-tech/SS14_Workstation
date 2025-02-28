using Content.Shared.MedicalScanner;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Content.Shared.RPSX.CCVars;
using Content.Client.UserInterface.Controls;

namespace Content.Client.HealthAnalyzer.UI
{
    [UsedImplicitly]
    public sealed class HealthAnalyzerBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private FancyWindow? _window;

        [Dependency] private readonly IConfigurationManager _cfg = default!;

        public HealthAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();
            if (!_cfg.GetCVar(RPSXCCVars.SurgeryEnabled))
            {
                _window = this.CreateWindow<HealthAnalyzerWindow>();
                _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;
            }
            else
            {
                _window = this.CreateWindow<SurgeryHealthAnalyzerWindow>();
                _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;
            }
        }

        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            if (_window == null)
                return;

            if (message is not HealthAnalyzerScannedUserMessage cast)
                return;

            switch (_window)
            {
                case SurgeryHealthAnalyzerWindow surgeryHealthAnalyzer:
                    surgeryHealthAnalyzer.Populate(cast);
                    break;
                case HealthAnalyzerWindow healthAnalyzerWindow:
                    healthAnalyzerWindow.Populate(cast);
                    break;
            }
        }
    }
}
