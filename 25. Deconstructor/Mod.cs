using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Assets;
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

        public StorageContainer storageContainer;
        public bool subscribed;

        public void Start()
        {
            storageContainer = GetComponent<StorageContainer>();
        }

        public void AddItem(InventoryItem item)
        {
            if (storageContainer.container.RemoveItem(item.item, true))
            {
                Destroy(item.item.gameObject);
                ITechData techData = CraftData.Get(item.item.GetTechType(), true);
                for (int i = 0; i < techData.ingredientCount; i++)
                {
                    IIngredient ingredient = techData.GetIngredient(i);
                    if (ingredient.techType == TechType.None) return;
                    CraftData.AddToInventory(ingredient.techType);
                }
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
            if (!subscribed)
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

        public class Prefab : ModPrefab
        {
            public Prefab() : base("itemdeconstructor", "itemdeconstructor", techType) { }

            public override GameObject GetGameObject()
            {
                var prefab = Resources.Load<GameObject>("submarine/build/trashcans");
                var obj = Instantiate(prefab);

                obj.GetComponent<Trashcan>().enabled = false;

                return obj;
            }
        }
    }

    public static class ExtensionMethods
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key)
        {
            if (!d.ContainsKey(key)) return default(TValue);
            d.TryGetValue(key, out TValue value);
            return value;
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