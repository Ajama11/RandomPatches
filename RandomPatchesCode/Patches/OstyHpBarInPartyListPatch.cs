using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using RandomPatches.RandomPatchesCode.Singletons;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch(typeof(NMultiplayerPlayerState))]
public static class OstyHpBarInPartyListPatch
{
    [HarmonyPatch(nameof(NMultiplayerPlayerState.UpdateHighlightedState))]
    [HarmonyPostfix]
    public static void UpdateHighlightedStatePostfix(NMultiplayerPlayerState __instance)
    {
        if (__instance._isHighlighted)
        {
            OstyHpPartyListSingleton.OstyHpBar[__instance].FadeInHpLabel(0.1f);
        }
        else
        {
            OstyHpPartyListSingleton.OstyHpBar[__instance].FadeOutHpLabel(0.5f, 0);
        }
    }
    
    [HarmonyPatch(nameof(NMultiplayerPlayerState.RefreshValues))]
    [HarmonyPostfix]
    public static void RefreshValuesPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(nameof(NMultiplayerPlayerState.BlockChanged))]
    [HarmonyPostfix]
    public static void BlockChangedPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(nameof(NMultiplayerPlayerState.OnCombatEnded))]
    [HarmonyPostfix]
    public static void OnCombatEndedPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.OstyHpBar[__instance].Visible = false;
    }
}