using System.Diagnostics.CodeAnalysis;
using BaseLib.Cards;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch(typeof(MyShinies))]
[SuppressMessage("ReSharper", "RedundantAssignment")]
public static class ReworkMyShiniesPatches
{
    // Example of nuking a card into the Event Rarity, if I ever wanna do that at some point
    // [HarmonyPatch(typeof(MyShinies), MethodType.Constructor)]
    // private static void Postfix(MyShinies __instance)
    // {
    //     AccessTools.Field(typeof (CardModel), "<Rarity>k__BackingField").SetValue(__instance, CardRarity.Event);
    // }

    [HarmonyPatch("get_CanonicalVars")]
    [HarmonyPrefix]
    private static bool CanonicalVarsPrefix(MyShinies __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result =
        [
            new DamageVar(30, DamageProps.card),
            new GoldVar(200)
        ];
        return false;
    }
    
    [HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]
    [HarmonyPrefix]
    private static bool CanonicalKeywordsPrefix(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (__instance is not MyShinies) return true;
        
        __result = [BaseLibKeywords.Purge];
        return false;
    }
    
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanBeGeneratedInCombat), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool HealingTagPrefix(CardModel __instance, ref bool __result)
    {
        if (__instance is not MyShinies) return true;
        
        __result = false;
        return false;
    }

    [HarmonyPatch("OnPlay")]
    [HarmonyPrefix]
    private static bool OnPlayPrefix(MyShinies __instance, ref Task __result, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        __result = MyOnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task MyOnPlay(MyShinies card, PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var player in card.Owner.RunState.Players)
            await PlayerCmd.GainGold(card.DynamicVars.Gold.BaseValue, player);
        
        await CommonActions.CardAttack(card, play,
                vfx: VfxCmd.coinExplosionJumboPath,
                tmpSfx: TmpSfx.heavyAttack)
            .Execute(choiceContext);
    }
    
    [HarmonyPatch("OnUpgrade")]
    [HarmonyPrefix]
    private static bool UpgradePrefix(MyShinies __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(10);
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}