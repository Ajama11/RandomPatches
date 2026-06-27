using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch]
public static class AveMujicaColorPatches
{
    private static readonly Color BrightColor = new ("7799cc");
    private static readonly Color DarkColor = new ("79799c");
    
    [HarmonyPatch(
        typeof(AveMujica.AveMujicaCode.Character.AveMujica),
        nameof(AveMujica.AveMujicaCode.Character.AveMujica.NameColor),
        MethodType.Getter
    )]
    internal static class ReplaceNameColor
    {
        internal static void Postfix(ref Color __result)
        {
            __result = BrightColor;
        }
    }

    // [HarmonyPatch(
    //     typeof(CharacterModel),
    //     nameof(CharacterModel.MapDrawingColor),
    //     MethodType.Getter
    // )]
    // internal static class ReplaceMapDrawingColor
    // {
    //     internal static void Postfix(ref Color __result, CharacterModel __instance)
    //     {
    //         if (__instance is AveMujica.AveMujicaCode.Character.AveMujica) __result = DarkColor;
    //     }
    // }
    
    [HarmonyPatch(
        typeof(CharacterModel),
        nameof(CharacterModel.DialogueColor),
        MethodType.Getter
    )]
    internal static class ReplaceDialogueColor
    {
        internal static void Postfix(ref Color __result, CharacterModel __instance)
        {
            if (__instance is AveMujica.AveMujicaCode.Character.AveMujica) __result = DarkColor;
        }
    }

    // [HarmonyPatch(
    //     typeof(AveMujica.AveMujicaCode.Character.AveMujicaCardPool),
    //     nameof(AveMujica.AveMujicaCode.Character.AveMujicaCardPool.DeckEntryCardColor),
    //     MethodType.Getter
    // )]
    // internal static class ReplaceDeckEntryCardColor
    // {
    //     internal static void Postfix(ref Color __result)
    //     {
    //         __result = BrightColor;
    //     }
    // }
}