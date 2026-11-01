using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Checksums;
using FileAccess = Godot.FileAccess;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch(typeof(ChecksumTracker), nameof(ChecksumTracker.LogStateDivergence))]
public static class StateDivergenceDumpPatch
{
    [HarmonyPrefix]
    public static void DumpIntoTxtFilesPrefix(ChecksumTracker.TrackedChecksum localChecksum, StateDivergenceMessage message)
    {
        using var localDump = FileAccess.Open("user://local_state_dump.txt", FileAccess.ModeFlags.Write);
        using var remoteDump = FileAccess.Open("user://remote_state_dump.txt", FileAccess.ModeFlags.Write);
        
        localDump.StoreString(localChecksum.fullState.ToString());
        remoteDump.StoreString(message.senderCombatState.ToString());
        
        localDump.Close(); // Just in case, I'm so sick of memory leaks
        remoteDump.Close();
    }
}