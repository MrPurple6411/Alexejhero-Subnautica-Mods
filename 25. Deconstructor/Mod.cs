using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

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

                obj.GetComponent<Trashcan>().enabled = false;
                obj.AddComponent<Deconstructor>();

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

            LanguageHandler.SetLanguageLine("UseDeconstructor", "DECONSTRUCTOR");
            LanguageHandler.SetLanguageLine("DeconstructorStorageLabel", "DECONSTRUCTOR");
        }

        /* ######################################################################### */

        public StorageContainer storageContainer;
        public bool subscribed;
        public List<DeconstructItem> timers = new List<DeconstructItem>();

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
            storageContainer = GetComponent<StorageContainer>();
            storageContainer.height = 10;
            storageContainer.width = 8;
        }
        public void Update()
        {
            List<DeconstructItem> toRemove = new List<DeconstructItem>();
            foreach (DeconstructItem item in timers)
            {
                item.Timer += Time.deltaTime;
                if (item.Timer >= 1)
                {
                    toRemove.Add(item);
                    if (storageContainer.container.RemoveItem(item.Item.item, true))
                    {
                        Destroy(item.Item.item.gameObject);
                        ITechData techData = CraftData.Get(item.Item.item.GetTechType(), true);
                        for (int i = 0; i < techData.ingredientCount; i++)
                        {
                            IIngredient ingredient = techData.GetIngredient(i);
                            if (ingredient.techType == TechType.None) return;
                            CraftData.AddToInventory(ingredient.techType);
                        }
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
            timers.Add(new DeconstructItem(item));
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
            if (!subscribed && storageContainer != null)
            {
                storageContainer.enabled = true;
                storageContainer.container.containerType = ItemsContainerType.Trashcan;
                storageContainer.container.onAddItem += AddItem;
                storageContainer.container.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
                subscribed = true;
            }
        }
        public void OnDisable()
        {
            if (subscribed)
            {
                storageContainer.container.onAddItem -= AddItem;
                storageContainer.container.isAllowedToAdd = null;
                storageContainer.enabled = false;
                subscribed = false;
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