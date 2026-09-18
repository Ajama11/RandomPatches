using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using RandomPatches.RandomPatchesCode.Singletons;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch]
public static class OstyHpBarInPartyListPatch
{
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.UpdateHighlightedState))]
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
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.RefreshValues))]
    [HarmonyPostfix]
    public static void RefreshValuesPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.BlockChanged))]
    [HarmonyPostfix]
    public static void BlockChangedPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.OnCombatEnded))]
    [HarmonyPostfix]
    public static void OnCombatEndedPostfix(NMultiplayerPlayerState __instance)
    {
        OstyHpPartyListSingleton.OstyHpBar[__instance].Visible = false;
    }
    
    [HarmonyPatch(
        typeof(CreatureCmd),
        nameof(CreatureCmd.LoseMaxHp),
        MethodType.Async)]
    [HarmonyPostfix]
    public static void LoseMaxHpPostfix()
    {
        foreach (var state in NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes)
        {
            OstyHpPartyListSingleton.UpdateOstyValues(state);
        }
    }
}