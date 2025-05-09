﻿using Content.Client.RPSX.Utils;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;

[GenerateTypedNameReferences]
public sealed partial class NarsiRitualsWindow : FancyWindow
{
    private readonly NarsiRitualsBoundInterface _bui;

    public NarsiRitualsWindow(NarsiRitualsBoundInterface bui)
    {
        RobustXamlLoader.Load(this);
        _bui = bui;
    }

    public void UpdateState(NarsiRitualsState state)
    {
        TabContainer.RemoveAllChildren();

        foreach (var (category, index) in state.RitualsCategories.WithIndex())
        {
            var control = new NarsiRitualsCategoryControl(_bui, category.Rituals);

            TabContainer.AddChild(control);
            TabContainer.SetTabTitle(index, category.Name);
        }


        switch (state.RitualsProgressState)
        {
            case NarsiRitualsProgressState.Idle:
                AltarStatus.Text = "Готов к проведению ритуала";
                AltarStatus.FontColorOverride = StyleNano.GoodGreenFore;
                break;
            case NarsiRitualsProgressState.Working:
                AltarStatus.Text = "Проводится ритуал...";
                AltarStatus.FontColorOverride = StyleNano.ConcerningOrangeFore;
                break;
            case NarsiRitualsProgressState.Delay:
                AltarStatus.Text = "Нар'Си какое-то время не слышит вас...";
                AltarStatus.FontColorOverride = StyleNano.DangerousRedFore;
                break;
        }
    }
}
