using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch]
public static class OtherAncientOptionPatch
{
    [HarmonyPatch(typeof(AncientEventModel), "GenerateInitialOptionsWrapper")]
    [HarmonyPostfix]
    public static void WrapperPostfix(AncientEventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner == null) return;
        
        var options = __instance._generatedOptions?.ToList();
        if (options == null) return;

        List<EventOption> sameAncientOptions = [];
        List<EventOption> otherAncientOptions = [];

        foreach (var ancient in ModelDb.AllAncients.Where(a => __instance.GetType() == a.GetType()))
        {
            sameAncientOptions.AddRange(CreateOptionsForAncient(ancient, __instance, options));
        }
        
        foreach (var ancient in ModelDb.AllAncients.Where(a => a is not Neow && __instance.GetType() != a.GetType()))
        {
            otherAncientOptions.AddRange(CreateOptionsForAncient(ancient, __instance));
        }

        if (sameAncientOptions.Count != 0)
            options.Add(__instance.Rng.NextItem(sameAncientOptions)!);
        
        if (otherAncientOptions.Count != 0)
            options.Add(__instance.Rng.NextItem(otherAncientOptions)!);
        
        __result = options;
    }

    public static List<EventOption> CreateOptionsForAncient(AncientEventModel ancient, AncientEventModel instance, List<EventOption>? denylist = null)
    {
        var mutableAncient = ancient.ToMutable();
        mutableAncient.Owner = instance.Owner;
        mutableAncient.Rng = instance.Rng;
            
        var generate = AccessTools.Method(mutableAncient.GetType(), "GenerateInitialOptions");
        if (generate == null) return [];

        List<EventOption> originalOptions = [];
        List<EventOption> newOptions = [];

        List<ModelId> exists = instance.Owner!.Relics
            .Where(r => r.Rarity == RelicRarity.Ancient)
            .Select(r => r.Id)
            .ToList();

        if (denylist != null)
        {
            exists.AddRange(denylist.Select(o => o.Relic!.Id));
        }

        for (int i = 0; i < 10; i++)
        {
            var newSetOfThree = (IReadOnlyList<EventOption>?) generate.Invoke(mutableAncient, null);
            if (newSetOfThree == null) continue;

            var relicsToAdd = newSetOfThree.Where(o =>
                    o.Relic != null &&
                    !exists.Contains(o.Relic.Id))
                .ToList();
                
            originalOptions.AddRange(relicsToAdd);
            exists.AddRange(relicsToAdd.Select(o => o.Relic!.Id));
        }
            
        foreach (var originalOption in originalOptions)
        {
            newOptions.Add(instance.RelicOption(originalOption.Relic!));
        }

        return newOptions;
    }
}