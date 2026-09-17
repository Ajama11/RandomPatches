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
        if (__instance._player == null) return true;
        if (__instance._player.Creature.Pets.Count == 0) return true;
        
        const string str = "RANDOMPATCHES-PET_HP";

        LocString title;
        LocString description;

        if (__instance._player.Creature.Pets.Count == 1)
        {
            if (__instance._player.Osty != null)
            {
                title = HoverTipFactory.L10NStatic(str + ".title_osty");
                description = HoverTipFactory.L10NStatic(str + ".description_osty");
        
                description.Add("Current", __instance._player.Osty.CurrentHp);
                description.Add("Max", __instance._player.Osty.MaxHp);
            }
            else if (__instance._player.Creature.Pets.Any(c => c.Monster?.GetType().FullName == OstyHpPartyListSingleton.Torchhead))
            {
                title = HoverTipFactory.L10NStatic(str + ".title_torchhead");
                description = HoverTipFactory.L10NStatic(str + ".description_torchhead");
        
                description.Add("Current", __instance._player.Creature.Pets[0].CurrentHp);
                description.Add("Max", __instance._player.Creature.Pets[0].MaxHp);
            }
            else
            {
                return true; // This is where other Osty-likes would go
            }
        }
        else
        {
            title = HoverTipFactory.L10NStatic(str + ".title_multiple");
            description = HoverTipFactory.L10NStatic(str + ".description_multiple");

            int ostyCurrent = 0;
            int ostyMax = 0;
            bool ostyPresent = false;
            
            if (__instance._player.Osty != null)
            {
                ostyCurrent = __instance._player.Osty.CurrentHp;
                ostyMax = __instance._player.Osty.MaxHp;
                ostyPresent = true;
            }
            
            description.Add("OstyCurrent", ostyCurrent);
            description.Add("OstyMax", ostyMax);
            description.Add("OstyPresent", ostyPresent);
            
            
            int torchheadCurrent = 0;
            int torchheadMax = 0;
            bool torchheadPresent = false;
            
            var torchhead = __instance._player.Creature.Pets.ToList()
                .Find(c => c.Monster?.GetType().FullName == OstyHpPartyListSingleton.Torchhead);
            if (torchhead != null)
            {
                torchheadCurrent = torchhead.CurrentHp;
                torchheadMax = torchhead.MaxHp;
                torchheadPresent = true;
            }
            
            description.Add("TorchheadCurrent", torchheadCurrent);
            description.Add("TorchheadMax", torchheadMax);
            description.Add("TorchheadPresent", torchheadPresent);
        }
        
        var tips = NHoverTipSet.CreateAndShow(__instance, new HoverTip(title, description));
        
        tips?.SetGlobalPosition(__instance.GlobalPosition + new Vector2(0, __instance.Size.Y + 20));

        return false;
    }
}