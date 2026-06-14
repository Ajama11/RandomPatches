using AveMujica.AveMujicaCode.Cards;
using AveMujica.AveMujicaCode.Cards.Uncommon;
using AveMujica.AveMujicaCode.Character;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using RandomPatches.RandomPatchesCode.Relics;

namespace RandomPatches.RandomPatchesCode.Relics;

[Pool(typeof(AveMujicaRelicPool))]
public class BluePlushie() : RandomPatchesRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(AveMujicaKeywords.Awaken)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState!.TurnNumber != 1) return;

        Flash();
        
        List<CardModel> bandMembers =
        [
            ModelDb.Card<Doloris>(),
            ModelDb.Card<Mortis>(),
            ModelDb.Card<Timoris>(),
            ModelDb.Card<Amoris>()
        ];

        CardModel card = CardFactory
            .GetDistinctForCombat(player, bandMembers, 1, Owner.RunState.Rng.CombatCardGeneration)
            .First();
        
        card.SetToFreeThisCombat();

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }
}