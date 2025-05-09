﻿using System.Collections.Generic;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.RPSX.DarkForces.Ratvar.UI;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.RPSX.DarkForces.Ratvar.Enchantment;

[GenerateTypedNameReferences]
public sealed partial class RatvarEnchantmentMenu : RadialMenu
{
    [Dependency] private readonly EntityManager _entManager = default!;

    private readonly SpriteSystem _spriteSystem;
    private RatvarEnchantmentBUI _bui;

    public RatvarEnchantmentMenu(RatvarEnchantmentBUI bui)
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        _spriteSystem = _entManager.System<SpriteSystem>();
        _bui = bui;
    }

    public void PopulateRadial(IReadOnlyCollection<EnchantmentUIModel> models)
    {
        Main.Children.Clear();
        foreach (var model in models)
        {
            var button = new RadialMenuTextureButton
            {
                StyleClasses = { "RadialMenuButton" },
                SetSize = new Vector2(64f, 64f),
                ToolTip = model.Name
            };

            var texture = new TextureRect
            {
                VerticalAlignment = VAlignment.Center,
                HorizontalAlignment = HAlignment.Center,
                Texture = _spriteSystem.Frame0(model.Icon),
                TextureScale = new Vector2(2f, 2f)
            };

            button.OnPressed += _ => _bui.Select(model.Id, model.Visuals);
            button.AddChild(texture);

            Main.AddChild(button);
        }
    }
}
