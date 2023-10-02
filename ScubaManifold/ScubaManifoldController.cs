namespace ScubaManifold;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

[HarmonyPatch]
internal static class ScubaManifoldController
{
    public static TechType ScubaManifoldTechType { get; internal set; }
    public static OxygenManager OxygenManager { get; private set; }
    public static ItemsContainer Container { get; private set; }
    public static Equipment Equipment { get; private set; }

    private const string tankSlot = "Tank";
    private static bool equipped = false;
    private static readonly List<Oxygen> sources = new();

    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    [HarmonyPostfix]
    private static void Player_Start_Postfix(Player __instance)
    {
        OxygenManager = __instance.oxygenMgr;
        Container = Inventory.main.container;
        Equipment = Inventory.main.equipment;

        List<InventoryItem> items = new();
        Container.GetItemTypes().ForEach(type => items.AddRange(Container.GetItems(type)));

        items.Where(item =>
        {
            Oxygen oxygen = item.item.gameObject.GetComponent<Oxygen>();
            if (oxygen != null)
            {
                sources.Add(oxygen);
                return true;
            }
            return false;
        });

        equipped = Equipment.GetItemInSlot("Tank")?.item?.GetTechType() == ScubaManifoldTechType;
        if (!equipped) return;

        sources.ForEach(OxygenManager.RegisterSource);

        Container.onAddItem += OnAddItem;
        Container.onRemoveItem += OnRemoveItem;
        Equipment.onEquip += OnEquip;
        Equipment.onUnequip += OnUnequip;
    }

    private static void OnUnequip(string slot, InventoryItem item)
    {
        if (slot != tankSlot || !equipped)
            return;

        equipped = false;
        sources.ForEach(OxygenManager.UnregisterSource);
    }

    private static void OnEquip(string slot, InventoryItem item)
    {
        if (slot != tankSlot)
            return;

        equipped = item?.item?.GetTechType() == ScubaManifoldTechType;
        if(equipped)
            sources.ForEach(OxygenManager.RegisterSource);
        else
            sources.ForEach(OxygenManager.UnregisterSource);
    }

    private static void OnRemoveItem(InventoryItem item)
    {
        Oxygen oxygen = item?.item?.gameObject?.GetComponent<Oxygen>();
        if (oxygen != null)
        {
            sources.Remove(oxygen);
            if(equipped)
                OxygenManager.UnregisterSource(oxygen);
        }
    }

    private static void OnAddItem(InventoryItem item)
    {
        Oxygen oxygen = item?.item?.gameObject?.GetComponent<Oxygen>();
        if (oxygen != null)
        {
            sources.Add(oxygen);
            if (equipped)
                OxygenManager.RegisterSource(oxygen);
        }
    }
}
