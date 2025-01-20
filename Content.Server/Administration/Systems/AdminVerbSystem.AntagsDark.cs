using Content.Server.RPSX.Bridges;
using Content.Shared.Database;
using Content.Shared.Mind;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Systems;

public sealed partial class AdminVerbSystem
{
    [Dependency] private readonly SharedMindSystem _minds = default!;
    [Dependency] private readonly IAntagBridge _antagBridge = default!;
    [Dependency] private readonly IVampireBridge _vampireBridge = default!;

    private void AddDarkStationAntags(GetVerbsEvent<Verb> args, ICommonSession player)
    {
        Verb narsiCult = new()
        {
            Text = "Сделать культистом Нар'Си",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Cult/Icons/cultist.rsi"), "cultist"),
            Act = () => _antagBridge.ForceMakeCultist(player),
            Impact = LogImpact.High,
            Message = "Делает цель культистом, также включает режим культа"
        };
        args.Verbs.Add(narsiCult);

        Verb narsiCultLeader = new()
        {
            Text = "Сделать лидером культа Нар'Си",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Cult/Icons/cultist.rsi"),
                "cultistLeader"),
            Act = () => _antagBridge.ForceMakeCultistLeader(player),
            Impact = LogImpact.High,
            Message = "Делает цель лидером культа, также включает режим культа"
        };
        args.Verbs.Add(narsiCultLeader);

        Verb vampire = new()
        {
            Text = "Сделать вампиром",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Vampire/icons.rsi"), "vampire"),
            Act = () => _antagBridge.ForceMakeVampire(player),
            Impact = LogImpact.High,
            Message = "Делает цель вампиром"
        };
        args.Verbs.Add(vampire);

        Verb ratvar = new()
        {
            Text = "Сделать праведником Ратвара",
            Category = VerbCategory.Antag,
            //Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/Misc/job_icons.rsi/HeadRevolutionary.png")),
            Act = () => _antagBridge.ForceMakeRatvarRighteous(player),
            Impact = LogImpact.High,
            Message = "Делает цель праведником ратвара, режим при этом не включается",
        };
        args.Verbs.Add(ratvar);
    }
}
