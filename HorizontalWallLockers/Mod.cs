namespace HorizontalWallLockers;

using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;
using static CraftData;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        CreateAndRRegisterHWL();
        Logger.LogInfo("Loaded!");
    }

    private static void CreateAndRRegisterHWL()
    {
        var hwl = new CustomPrefab("horizontalwalllocker", "Horizontal Wall Locker", "Small, wall-mounted storage solution.", SpriteManager.Get(TechType.SmallLocker));
        CraftDataHandler.SetRecipeData(hwl.Info.TechType, new RecipeData(new Ingredient(TechType.Titanium, 2)));
        if (GetBuilderIndex(TechType.SmallLocker, out var group, out var category, out _))
            hwl.SetPdaGroupCategoryAfter(group, category, TechType.SmallLocker);

        CloneTemplate hwlPrefab = new(hwl.Info, TechType.SmallLocker)
        {
            ModifyPrefab = (GameObject obj) => obj.FindChild("model").transform.rotation = Quaternion.Euler(0, 0, 90)
        };

        hwl.SetGameObject(hwlPrefab);

        hwl.Register();
    }
}