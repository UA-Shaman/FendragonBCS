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
        // якщо гравець нед≥йсний або в спектатор≥ Ч не св≥тимо
        if (__instance == null || !__instance.Alive || __instance.Player == null || __instance.Player.Entity == null || __instance.Player.WorldData.CurrentGameMode == EnumGameMode.Spectator)
        {
            return;
        }

        // Ѕеремо рюкзак
        IInventory inv = __instance?.Player?.InventoryManager?.GetOwnInventory(GlobalConstants.backpackInvClassName);
        if (inv == null) return;

        // ЎукаЇмо предмети твого мода, €к≥ можуть св≥тити
        ItemSlot backpackSlot = inv.FirstOrDefault(slot =>
        {
            if (slot.Empty) return false;

            // Ћише предмети твого мода
            if (slot.Itemstack.Collectible?.Code?.Domain != "fendragonbcs") return false;

            // ¬раховуЇмо toggle
            return slot.Itemstack.Attributes.GetAsBool("toggleBackpackLight", true);
        });

        if (backpackSlot == null) return;

        // ќтримуЇмо св≥тло в≥д цього предмета
        ItemStack stack = backpackSlot.Itemstack;
        byte[] backpackLight = stack?.Collectible?.GetLightHsv(__instance.World.BlockAccessor, __instance.SidedPos.AsBlockPos, stack);

        if (backpackLight == null) return;

        // “епер п≥дм≥н€Їмо св≥тло гравц€ на св≥тло з рюкзака, €кщо воно сильн≥ше за руки
        byte[] rightHandLight = __instance?.RightHandItemSlot?.Itemstack?.Collectible?.GetLightHsv(__instance.World.BlockAccessor, __instance.SidedPos.AsBlockPos, __instance.RightHandItemSlot.Itemstack);
        byte[] leftHandLight = __instance?.LeftHandItemSlot?.Itemstack?.Collectible?.GetLightHsv(__instance.World.BlockAccessor, __instance.SidedPos.AsBlockPos, __instance.LeftHandItemSlot.Itemstack);

        // якщо в руках н≥чого немаЇ або њх св≥тло слабше Ч використовуЇмо рюкзак
        if ((rightHandLight == null && leftHandLight == null)
            || (rightHandLight != null && rightHandLight[2] < backpackLight[2])
            || (leftHandLight != null && leftHandLight[2] < backpackLight[2]))
        {
            __result = backpackLight;
        }
    }
}
