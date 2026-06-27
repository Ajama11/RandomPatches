// using BaseLib.Utils;
// using MegaCrit.Sts2.Core.CardSelection;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Relics;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.RelicPools;
// using RandomPatches.RandomPatchesCode.Relics;
//
// namespace RandomPatches.RandomPatchesCode.Relics;
//
// [Pool(typeof(EventRelicPool))]
// public class GiantCookie() : RandomPatchesRelic
// {
//     public override RelicRarity Rarity =>
//         RelicRarity.Event;
//
//     public override async Task AfterObtained()
//     {
//         CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 0, 100)
//         {
//             Cancelable = true,
//             RequireManualConfirmation = true
//         };
//
//         foreach (CardModel card in await CardSelectCmd.FromDeckGeneric(Owner, prefs, c => c.IsUpgradable))
//         {
//             CardCmd.Upgrade(card);
//         }
//     }
// }