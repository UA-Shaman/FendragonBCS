using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace FendragonBCS;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigQuantitiesAndColors config)
    {
        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null || obj.Code == null)
            {
                continue;
            }

            foreach ((string key, QuantityAndColor value) in config.Backpacks)
            {
                if (!obj.WildCardMatch(AssetLocation.Create(key)))
                {
                    continue;
                }

                obj.Attributes ??= new JsonObject(new JObject());
                if (value.Quantity != null)
                {
                    obj.Attributes.Token["backpack"]["quantitySlots"] = JToken.FromObject(value.Quantity);
                }
                if (!string.IsNullOrEmpty(value.Color))
                {
                    obj.Attributes.Token["backpack"]["slotBgColor"] = JToken.FromObject(value.Color);
                }
            }
        }
    }
}