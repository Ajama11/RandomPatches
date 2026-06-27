// using MegaCrit.Sts2.Core.Context;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Powers;
// using MegaCrit.Sts2.Core.Models;
//
// namespace RandomPatches.RandomPatchesCode.Powers;
//
// public class TestPower : RandomPatchesPower
// {
//     public override PowerType Type => PowerType.Buff;
//     public override PowerStackType StackType => PowerStackType.Single;
//
//     public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
//     {
//         if (card.Owner.Creature != Owner)
//         {
//             modifiedCost = originalCost;
//             return false;
//         }
//         
//         MainFile.Logger.Info(
//             LocalContext.GetMe(card.CombatState)!.Character.Title.GetRawText() + 
//             " sees " + 
//             card.Id
//         );
//         
//         card.AddKeyword(CardKeyword.Exhaust); // This causes state divergence
//         
//         modifiedCost = originalCost;
//         return false;
//     }
// }