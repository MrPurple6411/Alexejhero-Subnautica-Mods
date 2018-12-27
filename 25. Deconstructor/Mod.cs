using Harmony;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;
using LoggedWhen = ModdingAdventCalendar.Utility.LoggedWhen;

namespace ModdingAdventCalendar.Deconstructor
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().Name;

                HarmonyInstance.Create("moddingadventcalendar.deconstructor").PatchAll(Assembly.GetExecutingAssembly());

                Deconstructor.Initialize();

                Console.WriteLine($"[{assembly}] Patched successfully!");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }

    public class Deconstructor : MonoBehaviour
    {
        public static TechType techType;
        public static TechData techData;

        public class Prefab : ModPrefab
        {
            public Prefab() : base("itemdeconstructor", "itemdeconstructor", techType) { }

            public override GameObject GetGameObject()
            {
                GameObject prefab = Resources.Load<GameObject>("submarine/build/trashcans");
                GameObject obj = Instantiate(prefab);

                StorageContainer storage = obj.GetComponent<StorageContainer>();
                storage.hoverText = "UseDeconstructor";
                storage.storageLabel = "DeconstructorStorageLabel";
                storage.preventDeconstructionIfNotEmpty = true;

                Trashcan trashcan = obj.GetComponent<Trashcan>();
                Destroy(trashcan);

                Deconstructor deconstructor = obj.AddComponent<Deconstructor>();

                return obj;
            }
        }

        public static void Initialize()
        {
            techType = TechTypeHandler.AddTechType("itemdeconstructor", "Deconstructor", "Deconstructs stuff", true);
            techData = new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 5),
                    new Ingredient(TechType.TitaniumIngot, 3),
                },
            };

            PrefabHandler.RegisterPrefab(new Prefab());
            CraftDataHandler.AddBuildable(techType);
            CraftDataHandler.AddToGroup(TechGroup.Miscellaneous, TechCategory.Misc, techType, TechType.LabTrashcan);
            CraftDataHandler.SetTechData(techType, techData);

            LanguageHandler.SetLanguageLine("UseDeconstructor", "Deconstructor");
            LanguageHandler.SetLanguageLine("DeconstructorStorageLabel", "DECONSTRUCTOR");
        }

        /* ######################################################################### */

        public StorageContainer storage;
        public bool subscribed;
        public List<DeconstructItem> timers = new List<DeconstructItem>();
        public static Dictionary<ItemsContainer, List<Pickupable>> dontDeconstruct = new Dictionary<ItemsContainer, List<Pickupable>>();

        public class DeconstructItem
        {
            public InventoryItem Item;
            public float Timer = 0;

            public DeconstructItem(InventoryItem item)
            {
                Item = item;
            }
        }

        public void Start()
        {
            storage = GetComponent<StorageContainer>();
            storage.height = 8;
            storage.width = 6;
            if (!dontDeconstruct.ContainsKey(storage.container))
                dontDeconstruct.Add(storage.container, new List<Pickupable>());
        }
        public void Update()
        {
            List<DeconstructItem> toRemove = new List<DeconstructItem>();
            foreach (DeconstructItem item in timers)
            {
                item.Timer += Time.deltaTime;
                if (item.Timer >= 3)
                {
                    toRemove.Add(item);
                    if (storage.container.RemoveItem(item.Item.item, true))
                    {
                        ITechData techData = CraftData.Get(item.Item.item.GetTechType(), true);
                        List<GameObject> toDestory = new List<GameObject>();
                        for (int i = 0; i < techData.ingredientCount; i++)
                        {
                            IIngredient ingredient = techData.GetIngredient(i);
                            if (ingredient.techType == TechType.None) return;
                            GameObject obj = AddToStorage(ingredient.techType);
                            toDestory.Add(obj);
                            if (obj == null) goto fail;
                        }
                        Destroy(item.Item.item.gameObject);
                        return;
                    fail:;
                        toDestory.Do(g => Destroy(g));
                        ErrorMessage.AddError("An unknown error has occurred.\nItem cannot be deconstructed");
                    }
                }
            }
            foreach (DeconstructItem item in toRemove)
            {
                timers.Remove(item);
            }
        }

        public void AddItem(InventoryItem item)
        {
            if (dontDeconstruct[storage.container].Contains(item.item))
            {
                dontDeconstruct[storage.container].Remove(item.item);
            }
            else
            {
                timers.Add(new DeconstructItem(item));
            }
        }

        public bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (pickupable.GetTechType() == TechType.None) return false;
            ITechData techData = CraftData.Get(pickupable.GetTechType(), true);
            if (techData == null) return false;
            if (techData.linkedItemCount != 0) return false;
            return true;
        }

        public void OnEnable()
        {
            if (!subscribed && storage != null)
            {
                storage.enabled = true;
                storage.container.containerType = (ItemsContainerType)1337;
                storage.container.onAddItem += AddItem;
                storage.container.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
                dontDeconstruct.Add(storage.container, new List<Pickupable>());
                subscribed = true;
            }
        }
        public void OnDisable()
        {
            if (subscribed)
            {
                storage.container.onAddItem -= AddItem;
                storage.container.isAllowedToAdd = null;
                storage.enabled = false;
                dontDeconstruct.Remove(storage.container);
                subscribed = false;
            }
        }

        public GameObject AddToStorage(TechType techType)
        {
            GameObject gameObject = CraftData.InstantiateFromPrefab(techType, false);
            if (gameObject == null) return null;
            gameObject.transform.position = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 3f;
            CrafterLogic.NotifyCraftEnd(gameObject, techType);
            Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.overrideTechUsed = true;
            pickupable.overrideTechType = techType;
            Inventory inventory = Inventory.main;
            if (pickupable == null || storage.container == null) return gameObject;
            dontDeconstruct[storage.container].Add(pickupable);
            bool x = !storage.container.HasRoomFor(pickupable);
            //bool y = storage.container.AddItem(pickupable) == null;
            bool y = CustomAddItem(storage.container, pickupable) == null;
            if (x || y)
            {
                dontDeconstruct[storage.container].Remove(pickupable);
                if (!inventory.HasRoomFor(pickupable) || !inventory.Pickup(pickupable))
                {
                    ErrorMessage.AddError(Language.main.Get("InventoryFull"));
                }
            }
            return gameObject;
        }
        public static void EditBackgroundSprite(uGUI_ItemIcon icon)
        {
            Atlas.Sprite sprite = SpriteManager.GetBackground(CraftData.BackgroundType.Normal);
            icon.GetInstanceMethod("CreateBackground").Invoke(icon, null);
            uGUI_Icon background = icon.GetInstanceField("background") as uGUI_Icon;
            background.sprite = sprite;
            background.enabled = true;
            background.color = new Color(1, 0, 0);
            Vector2 backgroundSize = (Vector2)icon.GetInstanceField("backgroundSize");
            icon.SetBackgroundSize(backgroundSize.x, backgroundSize.y, false);
            Material material = background.material;
            bool slice9Grid = sprite.slice9Grid;
            MaterialExtensions.SetKeyword(material, "SLICE_9_GRID", slice9Grid);
            if (slice9Grid)
            {
                material.SetVector(ShaderPropertyID._Size, (Vector2)icon.GetInstanceField("backgroundSize"));
            }
            icon.GetInstanceMethod("UpdateColor").Invoke(icon, null);
        }

        public static InventoryItem CustomAddItem(ItemsContainer container, Pickupable item)
        {
            InventoryItem inventoryItem = new InventoryItem(item);
            if (CustomAddItemCheck(container, inventoryItem))
            {
                return inventoryItem;
            }
            return null;
        }
        public static bool CustomAddItemCheck(ItemsContainer icontainer, InventoryItem item)
        {
            if (item == null)
            {
                return false;
            }
            Pickupable item2 = item.item;
            if (item2 == null)
            {
                return false;
            }
            if (!icontainer.HasRoomFor(item.item))
            {
                return false;
            }
            IItemsContainer container = item.container;
            if (container != null && !container.RemoveItem(item, false, true))
            {
                return false;
            }
            icontainer.UnsafeAdd(item);
            return true;
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(uGUI_ItemsContainer), "OnAddItem")]
#pragma warning disable IDE1006 // Naming Styles
        public static class uGUI_ItemsContainer_OnAddItem
#pragma warning restore IDE1006 // Naming Styles
        {
            [HarmonyPostfix]
            public static void Postfix(uGUI_ItemsContainer __instance, InventoryItem item)
            {
                ItemsContainer container = __instance.GetInstanceField("container") as ItemsContainer;
                string label = (string)container.GetInstanceField("_label");
                if (container.containerType == (ItemsContainerType)1337 && (label == "DECONSTRUCTOR" || label == "DeconstructorStorageLabel") && !Deconstructor.dontDeconstruct[container].Contains(item.item))
                {
                    Dictionary<InventoryItem, uGUI_ItemIcon> icons = __instance.GetInstanceField("items") as Dictionary<InventoryItem, uGUI_ItemIcon>;
                    uGUI_ItemIcon icon = icons[item];
                    Deconstructor.EditBackgroundSprite(icon);
                }
            }
        }
    }

    /*public static class Fabricator
    {
        public static List<TreeNode> addedNodes = new List<TreeNode>();
        public static Dictionary<TreeNode, ModCraftTreeLinkingNode> nodes = new Dictionary<TreeNode, ModCraftTreeLinkingNode>();
        public static CraftNode root;

        public static void Initialize()
        {
            CreateCustomFabricator();
        }

        public static void CreateCustomFabricator()
        {
            ModCraftTreeRoot root = CraftTreeHandler.CreateCustomCraftTreeAndType("deconstructorCraftTreeType", out CraftTree.Type type);
            // Add nodes
        }

        public static void AddRootNode(CraftNode node, ModCraftTreeRoot root)
        {
            
        }

        public static void AddNodesRecursively(CraftNode node, ModCraftTreeRoot root)
        {
            if (node.parent != null && !addedNodes.Contains(node.parent))
            {
                ModCraftTreeLinkingNode parent = GetCustomNodeForTreeNode(node.parent);
                if (parent == null)
                {
                    // Add it
                }
            }
        }

        public static ModCraftTreeLinkingNode GetCustomNodeForTreeNode(TreeNode node)
        {
            return nodes.Get(node);
        }
    }*/
}