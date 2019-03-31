// Code from https://github.com/PrimeSonic/PrimeSonicSubnauticaMods/blob/master/MoreCyclopsUpgrades/Modules/CyclopsModule.cs

using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.CyclopsInceptionUpgrade
{
    internal abstract class CyclopsModule : ModPrefab
    {
        private static readonly List<CyclopsModule> ModulesToPatch = new List<CyclopsModule>();

        private static readonly Dictionary<TechType, CyclopsModule> CyclopsModulesByTechType = new Dictionary<TechType, CyclopsModule>();

        public static TechType InceptionModuleID { get; protected set; } = TechType.UnusedOld; // Default value that shouldn't get hit

        public readonly string NameID;
        public readonly string FriendlyName;
        public readonly string Description;
        public readonly TechType RequiredForUnlock;
        public readonly CraftTree.Type Fabricator;
        public readonly string[] FabricatorTabs;

        protected readonly TechType PreFabTemplate;

        private readonly bool AddToCraftTree;

        protected CyclopsModule(string nameID, string friendlyName, string description, CraftTree.Type fabricator, string[] fabricatorTab, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
            : base(nameID, $"{nameID}Prefab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;
            Fabricator = fabricator;
            FabricatorTabs = fabricatorTab;

            AddToCraftTree = FabricatorTabs != null;
            PreFabTemplate = preFabTemplate;
        }

        protected CyclopsModule(string nameID, string friendlyName, string description, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
            : base(nameID, $"{nameID}Prefab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;

            AddToCraftTree = false;
            PreFabTemplate = preFabTemplate;
        }

        protected virtual void Patch()
        {
            TechType = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, RequiredForUnlock == TechType.None);

            if (RequiredForUnlock != TechType.None)
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[] { TechType }, $"{FriendlyName} blueprint discovered!");

            PrefabHandler.RegisterPrefab(this);

            SpriteHandler.RegisterSprite(TechType, Path.Combine(Assembly.GetExecutingAssembly().Location, $"Assets/{NameID}.png"));

            CraftDataHandler.SetTechData(TechType, GetRecipe());

            if (AddToCraftTree)
                CraftTreeHandler.AddCraftingNode(Fabricator, TechType, FabricatorTabs);

            CraftDataHandler.SetEquipmentType(TechType, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, TechType);

            SetStaticTechTypeID(TechType);
        }

        protected abstract void SetStaticTechTypeID(TechType techTypeID);

        protected abstract TechData GetRecipe();

        internal static void PatchAllModules(bool modulesEnabled)
        {
            ModulesToPatch.Add(new InceptionModule());

            foreach (CyclopsModule module in ModulesToPatch)
            {
                Logger.Log($"Patching {module.NameID}");
                module.Patch();
                CyclopsModulesByTechType.Add(module.TechType, module);
            }
        }

        public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
        {
            GameObject gameObject;

            if (techTypeID < TechType.Databox) // This is a standard upgrade module
            {
                gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));
            }
            else // Safety check in case these are disabled in the config
            {
                if (!CyclopsModulesByTechType.ContainsKey(techTypeID))
                    return null; // error condition

                // Get the CyclopsModule child class instance associated to this TechType
                CyclopsModule cyclopsModule = CyclopsModulesByTechType[techTypeID];

                // Instantiate a new prefab of the appripriate template TechType
                gameObject = cyclopsModule.GetGameObject();

                // Set the TechType value on the TechTag
                TechTag tag = gameObject.GetComponent<TechTag>();
                if (tag != null)
                    tag.type = techTypeID;
                else // Add if needed since this is how these are identified throughout the mod
                    gameObject.AddComponent<TechTag>().type = techTypeID;

                // Set the ClassId
                gameObject.GetComponent<PrefabIdentifier>().ClassId = cyclopsModule.NameID;
            }

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
