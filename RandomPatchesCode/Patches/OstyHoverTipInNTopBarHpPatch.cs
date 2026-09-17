using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.sts2.Core.Nodes.TopBar;
using RandomPatches.RandomPatchesCode.Singletons;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch(typeof(NTopBarHp))]
public static class OstyHoverTipInNTopBarHpPatch
{
    [HarmonyPatch("OnFocus")]
    [HarmonyPrefix]
    public static bool OnFocusPrefix(NTopBarHp __instance)
    {
        if (__instance._player?.Osty == null) return true;
        
        const string str = "RANDOMPATCHES-OSTY_HP";

        LocString title = HoverTipFactory.L10NStatic(str + ".title");
        LocString description = HoverTipFactory.L10NStatic(str + ".description");
        
        description.Add("Current", __instance._player.Osty.CurrentHp);
        description.Add("Max", __instance._player.Osty.MaxHp);

        var tips = NHoverTipSet.CreateAndShow(__instance, new HoverTip(title, description));
        
        tips?.SetGlobalPosition(__instance.GlobalPosition + new Vector2(0, __instance.Size.Y + 20));

        return false;
    }
}

[HarmonyPatch(typeof(NMultiplayerPlayerState))]
public static class OstyHpInPartyList
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