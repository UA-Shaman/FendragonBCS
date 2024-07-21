using Vintagestory.API.Common;

[assembly: ModInfo(name: "Fendragon Backpack Construction System", modID: "fendragonbcs", Side = "Universal")]

namespace FendragonBCS;

public class Core : ModSystem
{
    public static ConfigQuantitiesAndColors ConfigQuantitiesAndColors { get; private set; }

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