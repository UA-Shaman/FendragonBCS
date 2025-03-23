using HarmonyLib;
using Vintagestory.API.Common;

namespace FendragonBCS;

public class Core : ModSystem
{
    public string HarmonyID => Mod.Info.ModID;
    public Harmony HarmonyInstance => new(HarmonyID);

    public override void StartPre(ICoreAPI api)
    {
        HarmonyInstance.PatchAll();
    }

    public override void Dispose()
    {
        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }

    public ConfigQuantitiesAndColors ConfigQuantitiesAndColors { get; private set; }

    public static Core GetInstance(ICoreAPI api) => api.ModLoader.GetModSystem<Core>();

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side.IsServer())
        {
            ConfigQuantitiesAndColors = ModConfig.ReadConfig<ConfigQuantitiesAndColors>(api, $"FendragonBCSConfig.json");
            api.ApplyPatches(ConfigQuantitiesAndColors);
        }

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}