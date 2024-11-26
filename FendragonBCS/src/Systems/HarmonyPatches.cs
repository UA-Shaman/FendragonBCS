using HarmonyLib;
using Vintagestory.API.Common;

namespace FendragonBCS;

public class HarmonyPatches : ModSystem
{
    public string HarmonyID => Mod.Info.ModID;
    public Harmony HarmonyInstance => new(HarmonyID);

    public override void AssetsFinalize(ICoreAPI api)
    {
        HarmonyInstance.Patch(original: EntityPlayer_LightHsv_Patch.TargetMethod(), postfix: typeof(EntityPlayer_LightHsv_Patch).GetMethod(nameof(EntityPlayer_LightHsv_Patch.Postfix)));
    }

    public override void Dispose()
    {
        HarmonyInstance.Unpatch(original: EntityPlayer_LightHsv_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyID);
    }
}