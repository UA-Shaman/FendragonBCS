using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace FendragonBCS;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigQuantitiesAndColors config)
    {
        foreach ((string key, QuantityAndColor value) in config.Backpacks)
        {
            CollectibleObject obj = api.GetCollectible(key);

            if (obj == null || obj.Code == null || !obj.WildcardRegexMatch(key))
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

    public static CollectibleObject GetCollectible(this ICoreAPI api, string key)
    {
        return (CollectibleObject)api.World.GetBlock(new AssetLocation(key)) ?? api.World.GetItem(new AssetLocation(key));
    }

    public static bool WildcardRegexMatch(this CollectibleObject obj, string key)
    {
        return WildcardUtil.Match(new AssetLocation(key), obj.Code);
    }
}