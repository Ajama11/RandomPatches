using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace RandomPatches.RandomPatchesCode.Powers;

public class TestPower : RandomPatchesPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (card.Owner.Creature == Owner)
        {
            if (card.Keywords.Contains(CardKeyword.Exhaust) || card.Keywords.Contains(CardKeyword.Retain))
            {
                modifiedCost = 0M;
                return true;
            }
        }
        modifiedCost = originalCost;
        return false;
    }

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    { 
        return card.Owner == Owner.Player && 
               card.Type != CardType.Power &&
               keywords.Contains(CardKeyword.Retain) && 
               keywords.Add(CardKeyword.Exhaust);
    }
}