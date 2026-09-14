using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Rooms;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch]
public static class UnknownRestSitePatches
{
    [HarmonyPatch(typeof(NMapPointHistoryHoverTip), nameof(NMapPointHistoryHoverTip._Ready))]
    [HarmonyTranspiler]
    static List<CodeInstruction> RunHistoryTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo isRoomTypeRestSite = typeof(UnknownRestSitePatches).Method(nameof(IsRoomTypeRestSite));

        Label originalDefaultCaseLabel = generator.DefineLabel();
        Label myDefaultCaseLabel = generator.DefineLabel();
        Label successLabel = generator.DefineLabel();

        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldarg_0),  // 0: this
                new CodeMatch(OpCodes.Ldfld),    // 1: Load _entry
                new CodeMatch(OpCodes.Callvirt), // 2: get_Rooms()
                new CodeMatch(OpCodes.Call),     // 3: .First<>()
                new CodeMatch(OpCodes.Callvirt), // 4: get_RoomType()
                new CodeMatch(OpCodes.Stloc_S),  // 5: RoomType (V_4)
                
                new CodeMatch(OpCodes.Ldloc_S),  // 6: Load RoomType
                new CodeMatch(OpCodes.Ldc_I4_1), // 7: Load 1
                new CodeMatch(OpCodes.Sub),      // 8: RoomType - 1, for enum
                
                new CodeMatch(OpCodes.Switch),   // 9
                new CodeMatch(OpCodes.Br_S),     // 10: Go to default case
                
                new CodeMatch(OpCodes.Ldstr),    // 11: "ROOM_UNKNOWN_ENEMY"
                new CodeMatch(OpCodes.Stloc_2),  // 12: str3
                new CodeMatch(OpCodes.Br_S),     // 13: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 14: "ROOM_UNKNOWN_TREASURE"
                new CodeMatch(OpCodes.Stloc_2),  // 15: str3
                new CodeMatch(OpCodes.Br_S),     // 16: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 17: "ROOM_UNKNOWN_MERCHANT"
                new CodeMatch(OpCodes.Stloc_2),  // 18: str3
                new CodeMatch(OpCodes.Br_S),     // 19: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 20: "ROOM_UNKNOWN_ELITE"
                new CodeMatch(OpCodes.Stloc_2),  // 21: str3
                new CodeMatch(OpCodes.Br_S),     // 22: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 23: "ROOM_EVENT"
                new CodeMatch(OpCodes.Stloc_2),  // 24: str3
                new CodeMatch(OpCodes.Br_S),     // 25: Go to success case
                
                new CodeMatch(OpCodes.Ldnull),   // 26: Default case, load null
                new CodeMatch(OpCodes.Stloc_2),  // 27: str3
                
                new CodeMatch(OpCodes.Ldloc_2),  // 28: Success case, load str3
                new CodeMatch(OpCodes.Stloc_0),  // 29: str2
            ])
            .ThrowIfInvalid("UnknownRestSitePatches RunHistoryTranspiler could not find the correct position");

        var roomTypeOperand = matcher.InstructionAt(6).operand;
        
        matcher.InstructionAt(26).labels.Add(originalDefaultCaseLabel);
        matcher.InstructionAt(28).labels.Add(successLabel);
        
        matcher.InstructionAt(10).operand = myDefaultCaseLabel;
        matcher.InstructionAt(13).operand = successLabel; // ENEMY
        matcher.InstructionAt(16).operand = successLabel; // TREASURE
        matcher.InstructionAt(19).operand = successLabel; // MERCHANT
        matcher.InstructionAt(22).operand = successLabel; // ELITE
        matcher.InstructionAt(25).operand = successLabel; // EVENT

        matcher.Advance(26) // Beginning of default case
            .Insert([
                new CodeInstruction(OpCodes.Ldloc_S, roomTypeOperand),
                new CodeInstruction(OpCodes.Call, isRoomTypeRestSite),
                new CodeInstruction(OpCodes.Brfalse_S, originalDefaultCaseLabel),
                new CodeInstruction(OpCodes.Ldstr, "ROOM_UNKNOWN_REST"),
                new CodeInstruction(OpCodes.Stloc_2),
                new CodeInstruction(OpCodes.Br_S, successLabel)
            ]);

        matcher.Labels.Add(myDefaultCaseLabel);
        
        return matcher.InstructionEnumeration().ToList();
    }
    
    [HarmonyPatch(typeof(NTopBarRoomIcon), nameof(NTopBarRoomIcon.GetHoverTipPrefixForUnknownRoomType))]
    [HarmonyTranspiler]
    static List<CodeInstruction> TopBarTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo isRoomTypeRestSite = typeof(UnknownRestSitePatches).Method(nameof(IsRoomTypeRestSite));

        Label originalDefaultCaseLabel = generator.DefineLabel();
        Label myDefaultCaseLabel = generator.DefineLabel();
        Label successLabel = generator.DefineLabel();

        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Stloc_2),  // 0: RoomType (V_2)
                
                new CodeMatch(OpCodes.Ldloc_2),  // 1: Load RoomType
                new CodeMatch(OpCodes.Ldc_I4_1), // 2: Load 1
                new CodeMatch(OpCodes.Sub),      // 3: RoomType - 1, for enum
                
                new CodeMatch(OpCodes.Switch),   // 4
                new CodeMatch(OpCodes.Br_S),     // 5: Go to default case
                
                new CodeMatch(OpCodes.Ldstr),    // 6: "ROOM_UNKNOWN_ENEMY"
                new CodeMatch(OpCodes.Stloc_1),  // 7: V_1
                new CodeMatch(OpCodes.Br_S),     // 8: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 9: "ROOM_UNKNOWN_TREASURE"
                new CodeMatch(OpCodes.Stloc_1),  // 10: V_1
                new CodeMatch(OpCodes.Br_S),     // 11: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 12: "ROOM_UNKNOWN_MERCHANT"
                new CodeMatch(OpCodes.Stloc_1),  // 13: V_1
                new CodeMatch(OpCodes.Br_S),     // 14: Go to success case
                
                new CodeMatch(OpCodes.Ldstr),    // 15: "ROOM_UNKNOWN_EVENT"
                new CodeMatch(OpCodes.Stloc_1),  // 16: V_1
                new CodeMatch(OpCodes.Br_S),     // 17: Go to success case
                
                new CodeMatch(OpCodes.Newobj),   // 18: Default case
                new CodeMatch(OpCodes.Throw),    // 19
                
                new CodeMatch(OpCodes.Ldloc_1),  // 20: Success case, load V_1
                new CodeMatch(OpCodes.Ret),      // 21: Return V_1
            ])
            .ThrowIfInvalid("UnknownRestSitePatches TopBarTranspiler could not find the correct position");
        
        matcher.InstructionAt(18).labels.Add(originalDefaultCaseLabel);
        matcher.InstructionAt(20).labels.Add(successLabel);
        
        matcher.InstructionAt(5).operand = myDefaultCaseLabel;
        matcher.InstructionAt(8).operand = successLabel; // ENEMY
        matcher.InstructionAt(11).operand = successLabel; // TREASURE
        matcher.InstructionAt(14).operand = successLabel; // MERCHANT
        matcher.InstructionAt(17).operand = successLabel; // EVENT

        matcher.Advance(18) // Beginning of default case
            .Insert([
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, isRoomTypeRestSite),
                new CodeInstruction(OpCodes.Brfalse_S, originalDefaultCaseLabel),
                new CodeInstruction(OpCodes.Ldstr, "ROOM_UNKNOWN_REST"),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Br_S, successLabel)
            ]);

        matcher.Labels.Add(myDefaultCaseLabel);
        
        return matcher.InstructionEnumeration().ToList();
    }

    private static bool IsRoomTypeRestSite(RoomType roomType)
    {
        return roomType == RoomType.RestSite;
    }
}