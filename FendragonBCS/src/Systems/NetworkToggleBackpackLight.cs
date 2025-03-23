using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace FendragonBCS;

[ProtoContract]
public class ToggleBackpackLightRequest
{
}

public class NetworkToggleBackpackLight : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.Network
            .RegisterChannel("fendragonbcs:togglebackpacklight")
            .RegisterMessageType(typeof(ToggleBackpackLightRequest));
    }

    #region Client
    IClientNetworkChannel clientChannel;
    ICoreClientAPI clientApi;

    public override void StartClientSide(ICoreClientAPI api)
    {
        clientApi = api;
        clientChannel = api.Network
            .GetChannel("fendragonbcs:togglebackpacklight");

        api.Input.RegisterHotKey("fendragonbcs:togglebackpacklight", Lang.Get("fendragonbcs:hotkey-togglebackpacklight"), GlKeys.L, shiftPressed: true);
        api.Input.SetHotKeyHandler("fendragonbcs:togglebackpacklight", HandleToggleBackpackLight);
    }

    private bool HandleToggleBackpackLight(KeyCombination keyCombination)
    {
        clientChannel.SendPacket(new ToggleBackpackLightRequest());
        return true;
    }
    #endregion

    #region Server
    IServerNetworkChannel serverChannel;
    ICoreServerAPI serverApi;

    public override void StartServerSide(ICoreServerAPI api)
    {
        serverApi = api;
        serverChannel = api.Network
            .GetChannel("fendragonbcs:togglebackpacklight")
            .SetMessageHandler<ToggleBackpackLightRequest>(OnClientRequest);
    }

    private void OnClientRequest(IPlayer fromPlayer, ToggleBackpackLightRequest networkRequest)
    {
        IInventory inventory = fromPlayer.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName);
        if (inventory == null) return;

        IEnumerable<ItemSlot> slots = inventory.Where(slot => !slot.Empty && slot.Itemstack.Collectible.LightHsv.Any() && slot.Itemstack.Collectible.Code.Domain == "fendragonbcs");
        if (!slots.Any()) return;

        foreach (ItemSlot slot in slots)
        {
            bool newValue = !slot.Itemstack.Attributes.GetBool("toggleBackpackLight", true);
            slot.Itemstack.Attributes.SetBool("toggleBackpackLight", newValue);
            slot.MarkDirty();
        }
    }
    #endregion
}