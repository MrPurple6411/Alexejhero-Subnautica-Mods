using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using LoggedWhen = ModdingAdventCalendar.Utility.LoggedWhen;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.Deconstructor
{
    public static class Properties
    {
        public const string ID = "deconstructor";
        public const string NAME = "Deconstructor";
        public const string TOOLTIP = "Deconstructs stuff";

        public const int CRAFT_AMOUNT = 1;
        public static readonly TechType[] INGREDIENTS = new TechType[]
        {
            TechType.TitaniumIngot,
            TechType.Copper,
            TechType.JeweledDiskPiece,
            TechType.PrecursorIonCrystal
        };

        public const string ORIGINAL_PREFAB = "submarine/build/trashcans";

        public static readonly LanguageHelper HOVER_TEXT = "Deconstructor";
        public static readonly LanguageHelper STORAGE_LABEL = "DECONSTRUCTOR";

        public static readonly TechGroup TECH_GROUP = TechGroup.Miscellaneous;
        public static readonly TechCategory TECH_CATEGORY = TechCategory.Misc;
        public static readonly TechType INSERT_AFTER = TechType.LabTrashcan;

        public const int STORAGE_WIDTH = 6;
        public const int STORAGE_HEIGHT = 8;
    }

    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().Name;

                HarmonyInstance.Create("moddingadventcalendar.deconstructor").PatchAll(Assembly.GetExecutingAssembly());

                Deconstructor.Static.Initialize();

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
        public class Static
        {
            public class Prefab : PrefabHelper
            {
                public Prefab() : base(Properties.ID, Properties.ID, TECH_TYPE, () => GameObject()) { }

                public static GameObject GameObject()
                {
                    GameObject prefab = Resources.Load<GameObject>(Properties.ORIGINAL_PREFAB);
                    GameObject obj = Instantiate(prefab);

                    StorageContainer storage = obj.GetComponent<StorageContainer>();
                    storage.hoverText = Properties.HOVER_TEXT;
                    storage.storageLabel = Properties.STORAGE_LABEL;
                    storage.preventDeconstructionIfNotEmpty = true;
                    storage.height = Convert.ToInt32(Properties.STORAGE_HEIGHT);
                    storage.width = Convert.ToInt32(Properties.STORAGE_WIDTH);
                    storage.container.containerType = (ItemsContainerType)1337;

                    Trashcan trashcan = obj.GetComponent<Trashcan>();
                    Destroy(trashcan);

                    Deconstructor deconstructor = obj.AddComponent<Deconstructor>();

                    return obj;
                }
            }

            public static readonly BuildableHelper TECH_TYPE = new BuildableHelper(Properties.ID, Properties.NAME, Properties.TOOLTIP, Properties.TECH_GROUP, Properties.TECH_CATEGORY, Properties.INSERT_AFTER);
            public static readonly TechDataHelper TECH_DATA = new TechDataHelper(TECH_TYPE, Properties.CRAFT_AMOUNT, Properties.INGREDIENTS);
            public static readonly Prefab PREFAB = new Prefab();

            public static void Initialize()
            {
                
            }
        }
        
        public class Recipe
        {
            public TechType Result;
            public TechType[] OtherItems;

            public Recipe(TechType result, params TechType[] otherItems)
            {
                Result = result;
                OtherItems = otherItems;
            }
        }

        public StorageContainer storage;
        public bool subscribed;
        public List<Pickupable> itemsAddedByPlayer = new List<Pickupable>();
        public List<Pickupable> itemsDeconstructed = new List<Pickupable>();
        public Recipe validRecipe;
        public float timer = -1;

        public void Start() { }
        public void Update()
        {
            Console.WriteLine(1);
            if (storage.container.count == 0) return;
            foreach (InventoryItem item in storage.container)
            {
                Console.WriteLine(2);
                if (!itemsDeconstructed.Contains(item.item) && !itemsAddedByPlayer.Contains(item.item))
                {
                    itemsAddedByPlayer.Add(item.item);
                    Console.WriteLine("Added a(n) " + item.item.GetTechType().AsString() + " to the itemsAddedByPlayer list!");
                }
            }
            Console.WriteLine(3);
            validRecipe = null;
            foreach (Pickupable item in itemsAddedByPlayer)
            {
                Console.WriteLine(4);
                Console.WriteLine("Running foreach loop on a " + item.GetTechType().AsString());
                if (itemsDeconstructed.Count != 0) break;
                ITechData techData = CraftData.Get(item.GetTechType());
                Console.WriteLine(5);
                List<TechType> otherItems = itemsAddedByPlayer.Where(p => p != item).Select(p => p.GetTechType()).ToList();
                Console.WriteLine(6);
                Console.Write("otherItems list contains the following items: ");
                otherItems.Do(t => Console.Write(t.AsString() + ", "));
                Console.WriteLine();
                for (int i = 0; i < techData.linkedItemCount; i++)
                {
                    Console.WriteLine(7);
                    TechType linkedItem = techData.GetLinkedItem(i);
                    if (otherItems.Contains(linkedItem)) otherItems.Remove(linkedItem);
                    else goto @continue;
                }
                Console.WriteLine(8);
                for (int i = 0; i < techData.craftAmount - 1; i++)
                {
                    Console.WriteLine(9);
                    if (otherItems.Contains(item.GetTechType())) otherItems.Remove(item.GetTechType());
                }
                Console.WriteLine(10);
                if (otherItems.Count == 0)
                {
                    if (timer == -1) timer = 0;
                    // if (!red) makeRed();
                    validRecipe = new Recipe(item.GetTechType(), itemsAddedByPlayer.Where(p => p != item).Select(p => p.GetTechType()).ToArray());
                    Console.WriteLine(11);
                    break;
                }
            @continue:;
                continue;
            }
            if (validRecipe == null)
            {
                if (timer != -1) timer = -1;
                // if (red) makeNormal();
            }
            else timer += Time.deltaTime;
            if (timer >= 3)
            {
                foreach (Pickupable item in itemsAddedByPlayer)
                {
                    storage.container.RemoveItem(item, true);
                }
                ITechData techData = CraftData.Get(validRecipe.Result, true);
                List<GameObject> toDestory = new List<GameObject>();
                for (int i = 0; i < techData.ingredientCount; i++)
                {
                    IIngredient ingredient = techData.GetIngredient(i);
                    if (ingredient.techType == TechType.None) continue;
                    GameObject obj = AddToStorage(ingredient.techType);
                    // Add to itemsDeconstructed
                    toDestory.Add(obj);
                    if (obj == null) goto fail;
                }
            fail:;
                toDestory.Do(g => Destroy(g));
                itemsDeconstructed.RemoveAll(p => true);
                ErrorMessage.AddError("An unknown error has occurred.\nItem cannot be deconstructed");
            }
        }

        public void OnEnable()
        {
            if (!subscribed && storage != null)
            {
                storage.enabled = true;
                //storage.container.onAddItem += AddItem;
                storage.container.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
                subscribed = true;
            }
        }
        public void OnDisable()
        {
            if (subscribed)
            {
                //storage.container.onAddItem -= AddItem;
                storage.container.isAllowedToAdd = null;
                storage.enabled = false;
                subscribed = false;
            }
        }

        public bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (pickupable.GetTechType() == TechType.None) return false;
            ITechData techData = CraftData.Get(pickupable.GetTechType(), true);
            if (techData == null) return false;
            return true;
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
            bool x = !storage.container.HasRoomFor(pickupable);
            //bool y = storage.container.AddItem(pickupable) == null;
            bool y = CustomAddItem(storage.container, pickupable) == null;
            if (x || y)
            {
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

    /*public static class Patches
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
                if (container.containerType == (ItemsContainerType)1337 && (label == "DECONSTRUCTOR" || label == "DeconstructorStorageLabel") && !OldDeconstructor.dontDeconstruct[container].Contains(item.item))
                {
                    Dictionary<InventoryItem, uGUI_ItemIcon> icons = __instance.GetInstanceField("items") as Dictionary<InventoryItem, uGUI_ItemIcon>;
                    uGUI_ItemIcon icon = icons[item];
                    OldDeconstructor.EditBackgroundSprite(icon);
                    void test()
                    {

                    }
                }
            }
        }
    }*/

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