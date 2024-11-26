using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace FendragonBCS;

public class ConfigQuantitiesAndColors : IModConfig
{
    public Dictionary<string, QuantityAndColor> Backpacks { get; set; } = new();

    public ConfigQuantitiesAndColors(ICoreAPI api, ConfigQuantitiesAndColors previousConfig = null)
    {
        if (previousConfig != null)
        {
            Backpacks.AddRange(previousConfig.Backpacks);
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
    {
        foreach (CollectibleObject obj in api.World.Collectibles.Where(x => x.Code.Domain == "fendragonbcs" && x.Attributes != null && x.Attributes.KeyExists("backpack")))
        {
            if (!Backpacks.ContainsKey(obj.Code.ToString()))
            {
                QuantityAndColor qc = new QuantityAndColor(obj.Attributes["backpack"]?["quantitySlots"]?.AsInt(), obj.Attributes["backpack"]?["slotBgColor"]?.AsString());
                Backpacks.Add(obj.Code.ToString(), qc);
            }
        }
    }
}

public class QuantityAndColor
{
    public int? Quantity { get; set; }
    public string Color { get; set; }

    public QuantityAndColor(int? quantity, string color)
    {
        Quantity = quantity;
        Color = color;
    }
}