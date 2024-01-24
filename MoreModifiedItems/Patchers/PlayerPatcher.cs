namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using MoreModifiedItems.BasicEquipment;
using System.Collections.Generic;

[HarmonyPatch(typeof(Player))]
internal static class PlayerPatcher
{
    public static OxygenManager OxygenManager { get; private set; }
    public static ItemsContainer Container { get; private set; }
    public static Equipment Equipment { get; private set; }

    private const string tankSlot = "Tank";
    private static bool equipped = false;
    private static readonly List<Oxygen> sources = new();

    [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
    internal static void Player_Start_Postfix(Player __instance)
    {
        OxygenManager = __instance.oxygenMgr;
        Container = Inventory.main.container;
        Equipment = Inventory.main.equipment;

        List<InventoryItem> items = new();
        Container.GetItemTypes().ForEach(type => items.AddRange(Container.GetItems(type)));

        items.Do(item =>
        {
            Oxygen oxygen = item.item.gameObject.GetComponent<Oxygen>();
            if (oxygen != null)
            {
                sources.Add(oxygen);
            }
        });

        Container.onAddItem += OnAddItem;
        Container.onRemoveItem += OnRemoveItem;
        Equipment.onEquip += OnEquip;
        Equipment.onUnequip += OnUnequip;

        TechType techType = Equipment.GetTechTypeInSlot(tankSlot);
        equipped = techType == ScubaManifold.Instance.Info.TechType;

        Plugin.Log.LogDebug($"Equipped: {techType} == {ScubaManifold.Instance.Info.TechType}? {equipped}");
        if (!equipped) return;

        sources.ForEach(OxygenManager.RegisterSource);
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

        var techType = item?.item?.GetTechType();
        equipped = techType == ScubaManifold.Instance.Info.TechType;
        Plugin.Log.LogDebug($"Equipped: {techType} == {ScubaManifold.Instance.Info.TechType}? {equipped}");
        if (equipped)
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
            if (equipped)
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
