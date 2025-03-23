using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace FendragonBCS;

[HarmonyPatch(typeof(EntityPlayer), nameof(EntityPlayer.LightHsv), MethodType.Getter)]
public static class EntityPlayer_LightHsv_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref byte[] __result, EntityPlayer __instance)
    {
        if (__instance == null || !__instance.Alive || __instance.Player == null || __instance.Player.Entity == null || __instance.Player.WorldData.CurrentGameMode == EnumGameMode.Spectator)
        {
            return;
        }

        IInventory inv = __instance?.Player?.InventoryManager?.GetOwnInventory(GlobalConstants.backpackInvClassName);
        if (inv == null)
        {
            return;
        }
        IEnumerable<ItemSlot> slots = inv.Where(slot => !slot.Empty && slot.Itemstack.Collectible.LightHsv.Any() && slot.Itemstack.Collectible.Code.Domain == "fendragonbcs");
        if (!slots.Any())
        {
            return;
        }

        ItemStack righthandStack = __instance?.RightHandItemSlot?.Itemstack;
        ItemStack lefthandStack = __instance?.LeftHandItemSlot?.Itemstack;
        ItemStack firstBackpackStack = slots?.FirstOrDefault(slot => slot.Itemstack.Attributes.GetAsBool("toggleBackpackLight", true), null)?.Itemstack;

        if (firstBackpackStack == null) return;

        byte[] rightHandBytes = righthandStack?.Collectible?.GetLightHsv(__instance?.World?.BlockAccessor, null, righthandStack);
        byte[] leftHandBytes = lefthandStack?.Collectible?.GetLightHsv(__instance?.World?.BlockAccessor, null, lefthandStack);
        byte[] backpackBytes = firstBackpackStack?.Collectible?.GetLightHsv(__instance?.World?.BlockAccessor, null, firstBackpackStack);

        if (backpackBytes == null) return;

        if (rightHandBytes == null && leftHandBytes == null
        || rightHandBytes != null && rightHandBytes[2] < backpackBytes[2]
        || leftHandBytes != null && leftHandBytes[2] < backpackBytes[2])
        {
            __result = backpackBytes;
            return;
        }
    }
}