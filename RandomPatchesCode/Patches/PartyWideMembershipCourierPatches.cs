using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RandomPatches.RandomPatchesCode.Patches;

[HarmonyPatch]
public static class PartyWideMembershipCourierPatches
{
    [HarmonyPatch(typeof(MembershipCard), nameof(MembershipCard.ModifyMerchantPrice))]
    [HarmonyPrefix]
    public static bool MembershipCardPrefix(MembershipCard __instance, ref decimal __result, decimal originalPrice)
    {
        __result = originalPrice * (__instance.DynamicVars[MembershipCard._discountKey].BaseValue / 100);
        
        return false;
    }
    
    [HarmonyPatch(typeof(TheCourier), nameof(TheCourier.ModifyMerchantPrice))]
    [HarmonyPrefix]
    public static bool TheCourierPricePrefix(TheCourier __instance, ref decimal __result, decimal originalPrice)
    {
        __result = originalPrice * (1 - __instance.DynamicVars[TheCourier._discountKey].BaseValue / 100);
        
        return false;
    }
    
    [HarmonyPatch(typeof(TheCourier), nameof(TheCourier.ShouldRefillMerchantEntry))]
    [HarmonyPrefix]
    public static bool TheCourierRefillPrefix(TheCourier __instance, ref bool __result)
    {
        __result = true;
        
        return false;
    }
}