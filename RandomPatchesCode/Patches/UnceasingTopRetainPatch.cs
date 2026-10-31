using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch(typeof(UnceasingTop), nameof(UnceasingTop.AfterHandEmptied))]
public static class UnceasingTopRetainPatch
{
    [HarmonyPrefix]
    public static bool AfterHandEmptiedPrefix(ref Task __result, UnceasingTop __instance, PlayerChoiceContext choiceContext, Player player)
    {
        __result = Replacement(__instance, choiceContext, player);
        return false;
    }

    public static async Task Replacement(UnceasingTop instance, PlayerChoiceContext choiceContext, Player player)
    {
        if (player == instance.Owner && UnceasingTop.IsValidPhase(player.PlayerCombatState!.Phase))
        {
            instance.Flash();
            var card = await CardPileCmd.Draw(choiceContext, player);
            card?.GiveSingleTurnRetain();
        }
    }
}