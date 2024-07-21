using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace FendragonBCS;

public static class EntityPlayer_LightHsv_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(EntityPlayer), nameof(EntityPlayer.LightHsv));
    }

    public static void Postfix(ref byte[] __result, EntityPlayer __instance)
    {
        if (__instance == null || !__instance.Alive || __instance.Player == null || __instance.Player.WorldData.CurrentGameMode == EnumGameMode.Spectator)
        {
            return;
        }

        IInventory inv = __instance.Player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName);
        if (inv == null)
        {
            return;
        }
        IEnumerable<ItemSlot> slots = inv.Where(slot => !slot.Empty && slot.Itemstack.Collectible.LightHsv.Any() && slot.Itemstack.Collectible.Code.Domain == "fendragonbcs");
        if (!slots.Any())
        {
            return;
        }
        byte[] lightBytes = slots.First().Itemstack.Collectible.GetLightHsv(__instance.World.BlockAccessor, null, slots.First().Itemstack);
        __result = lightBytes;
        return;
    }
}