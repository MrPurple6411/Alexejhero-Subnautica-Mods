using Harmony;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AlexejheroYTB.CompressedTitanium
{
    public static class Mod
    {
        public static bool Patched = false;

        public static ModCraftTreeLinkingNode CompressTab;
        public static ModCraftTreeLinkingNode DecompressTab;

        public static readonly TechType TI_1 = TechType.Titanium;
        public static readonly TechType TI_10 = TechType.TitaniumIngot;
        public static TechType TI_100;
        public static TechType TI_1000;
        public static TechType TI_10000;
        public static TechType TI_100000;
        public static TechType TI_1000000;

        public static readonly Atlas.Sprite TI_1_Sprite = SpriteManager.Get(TechType.Titanium);
        public static readonly Atlas.Sprite TI_10_Sprite = SpriteManager.Get(TechType.TitaniumIngot);
        public static readonly Atlas.Sprite TI_100_Sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Assembly.GetExecutingAssembly().Location, "../Assets/compressed.png"));
        public static readonly Atlas.Sprite TI_1000_Sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Assembly.GetExecutingAssembly().Location, "../Assets/doublecompressed.png"));
        public static readonly Atlas.Sprite TI_10000_Sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Assembly.GetExecutingAssembly().Location, "../Assets/triplecompressed.png"));
        public static readonly Atlas.Sprite TI_100000_Sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Assembly.GetExecutingAssembly().Location, "../Assets/quadruplecompressed.png"));
        public static readonly Atlas.Sprite TI_1000000_Sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Assembly.GetExecutingAssembly().Location, "../Assets/singularitycompressed.png"));

        public static void Patch()
        {
            if (Patched) return;
            Patched = true;

            LoadFabricator();
            OverrideDefaults();
            LoadItems();
        }

        public static void LoadFabricator()
        {
            ModCraftTreeRoot Root = CraftTreeHandler.CreateCustomCraftTreeAndType("compressor", out CraftTree.Type Compressor);
            
            if (DefabricatorPresent())
            {
                CompressTab = Root;
                DecompressTab = null;
            }
            else
            {
                CompressTab = Root.AddTabNode("CompressedTitaniumCompress", "Compress", TI_1000000_Sprite);
                DecompressTab = Root.AddTabNode("CompressedTitaniumDecompress", "Decompress", TI_1_Sprite);
            }

            new Fabricator().Patch(Compressor);
        }

        public static void OverrideDefaults()
        {
            LanguageHandler.SetTechTypeTooltip(TI_1, "1 Titanium");
            LanguageHandler.SetTechTypeTooltip(TI_10, "10 Titanium");

            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, "Resources", "BasicMaterials", "TitaniumIngot");
            CompressTab.AddCraftingNode(TI_10);

            TechType TI_10_Decompress = TechTypeHandler.AddTechType("titaniumIngotDecompress", "Titanium", "1 Titanium", TI_1_Sprite);
            TechData TI_10_Decompress_TechData = new TechData(new Ingredient(TI_10, 1)) { craftAmount = 0, LinkedItems = TI_1.Repeat(10) };
            CraftDataHandler.SetTechData(TI_10_Decompress, TI_10_Decompress_TechData);
            DecompressTab?.AddCraftingNode(TI_10_Decompress);
        }

        public static void LoadItems()
        {
            #region Compressed Titanium Ingot

            TI_100 = TechTypeHandler.AddTechType($"compressedTitaniumIngot", "Compressed Titanium Ingot", "100 Titanium", TI_100_Sprite);
            TechData TI_100_TechData = new TechData(new Ingredient(TI_10, 10));
            CraftDataHandler.SetTechData(TI_100, TI_100_TechData);
            CraftDataHandler.SetCraftingTime(TI_100, 10);

            TechType TI_100_Decompress = TechTypeHandler.AddTechType("compressedTitaniumIngotDecompress", "Titanium Ingot", "10 Titanium", TI_10_Sprite);
            TechData TI_100_Decompress_TechData = new TechData(new Ingredient(TI_100, 1)) { craftAmount = 0, LinkedItems = TI_10.Repeat(10) };
            CraftDataHandler.SetTechData(TI_100_Decompress, TI_100_Decompress_TechData);
            CraftDataHandler.SetCraftingTime(TI_100_Decompress, 10);

            #endregion

            #region Double Compressed Titanium Ingot

            TI_1000 = TechTypeHandler.AddTechType($"doubleCompressedTitaniumIngot", "Double Compressed Titanium Ingot", "1.000 Titanium", TI_1000_Sprite);
            TechData TI_1000_TechData = new TechData(new Ingredient(TI_100, 10));
            CraftDataHandler.SetTechData(TI_1000, TI_1000_TechData);
            CraftDataHandler.SetCraftingTime(TI_1000, 15);

            TechType TI_1000_Decompress = TechTypeHandler.AddTechType("doubleCompressedTitaniumIngotDecompress", "Compressed Titanium Ingot", "100 Titanium", TI_100_Sprite);
            TechData TI_1000_Decompress_TechData = new TechData(new Ingredient(TI_1000, 1)) { craftAmount = 0, LinkedItems = TI_100.Repeat(10) };
            CraftDataHandler.SetTechData(TI_1000_Decompress, TI_1000_Decompress_TechData);
            CraftDataHandler.SetCraftingTime(TI_1000_Decompress, 15);

            #endregion

            #region Triple Compressed Titanium Ingot

            TI_10000 = TechTypeHandler.AddTechType($"tripleCompressedTitaniumIngot", "Triple Compressed Titanium Ingot", "10.000 Titanium", TI_10000_Sprite);
            TechData TI_10000_TechData = new TechData(new Ingredient(TI_1000, 10));
            CraftDataHandler.SetTechData(TI_10000, TI_10000_TechData);
            CraftDataHandler.SetCraftingTime(TI_10000, 20);

            TechType TI_10000_Decompress = TechTypeHandler.AddTechType("tripleCompressedTitaniumIngotDecompress", "Double Compressed Titanium Ingot", "1.000 Titanium", TI_1000_Sprite);
            TechData TI_10000_Decompress_TechData = new TechData(new Ingredient(TI_10000, 1)) { craftAmount = 0, LinkedItems = TI_1000.Repeat(10) };
            CraftDataHandler.SetTechData(TI_10000_Decompress, TI_10000_Decompress_TechData);
            CraftDataHandler.SetCraftingTime(TI_10000_Decompress, 20);

            #endregion

            #region Quadruple Compressed Titanium Ingot

            TI_100000 = TechTypeHandler.AddTechType($"quadrupleCompressedTitaniumIngot", "Quadruple Compressed Titanium Ingot", "100.000 Titanium", TI_100000_Sprite);
            TechData TI_100000_TechData = new TechData(new Ingredient(TI_10000, 10));
            CraftDataHandler.SetTechData(TI_100000, TI_100000_TechData);
            CraftDataHandler.SetCraftingTime(TI_100000, 25);

            TechType TI_100000_Decompress = TechTypeHandler.AddTechType("quadrupleCompressedTitaniumIngotDecompress", "Triple Compressed Titanium Ingot", "10.000 Titanium", TI_10000_Sprite);
            TechData TI_100000_Decompress_TechData = new TechData(new Ingredient(TI_100000, 1)) { craftAmount = 0, LinkedItems = TI_10000.Repeat(10) };
            CraftDataHandler.SetTechData(TI_100000_Decompress, TI_100000_Decompress_TechData);
            CraftDataHandler.SetCraftingTime(TI_100000_Decompress, 25);

            #endregion

            #region Titanium Ingot Singularity

            TI_1000000 = TechTypeHandler.AddTechType($"singularityCompressedTitaniumIngot", "Titanium Ingot Singularity", "1.000.000 Titanium", TI_1000000_Sprite);
            TechData TI_1000000_TechData = new TechData(new Ingredient(TI_100000, 10));
            CraftDataHandler.SetTechData(TI_1000000, TI_1000000_TechData);
            CraftDataHandler.SetCraftingTime(TI_1000000, 30);

            TechType TI_1000000_Decompress = TechTypeHandler.AddTechType("singularityCompressedTitaniumIngotDecompress", "Quadruple Compressed Titanium Ingot", "100.000 Titanium", TI_100000_Sprite);
            TechData TI_1000000_Decompress_TechData = new TechData(new Ingredient(TI_1000000, 1)) { craftAmount = 0, LinkedItems = TI_100000.Repeat(10) };
            CraftDataHandler.SetTechData(TI_1000000_Decompress, TI_1000000_Decompress_TechData);
            CraftDataHandler.SetCraftingTime(TI_1000000_Decompress, 30);

            #endregion

            CompressTab.AddCraftingNode(TI_100, TI_1000, TI_10000, TI_100000, TI_1000000);
            DecompressTab?.AddCraftingNode(TI_100_Decompress, TI_1000_Decompress, TI_10000_Decompress, TI_100000_Decompress, TI_1000000_Decompress);
        }

        public static bool DefabricatorPresent()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.GetName().Name == "AgonyDefabricator") return true;
            return false;
        }
    }

    public class Fabricator : ModPrefab
    {
        public CraftTree.Type TreeTypeID { get; set; }

        public Fabricator() : base("CompressorFab", $"CompressorFabPrefab", TechTypeHandler.AddTechType("CompressorFabTT", "Compressor", "A machine which allows you to compress resources.\n\nIf PVD's \"Defabricator\" mod is not present, you can also decompress items using this fabricator."))
        {
        }

        public void Patch(CraftTree.Type craftTreeType)
        {
            TreeTypeID = craftTreeType;

            CraftDataHandler.SetTechData(TechType, new TechData()
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Diamond, 1),
                    new Ingredient(TechType.Lead, 1),
                }
            });

            CraftDataHandler.AddBuildable(TechType);
            CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, TechType, TechType.Fabricator);

            LanguageHandler.SetLanguageLine("UseCompressor", "Use Vehicle Module Fabricator");

            PrefabHandler.RegisterPrefab(this);
        }

        public override GameObject GetGameObject()
        {
            var prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator"));

            GameObject fabLight = prefab.FindChild("fabricatorLight");
            GameObject fabModel = prefab.FindChild("submarine_fabricator_03");

            PrefabIdentifier prefabId = prefab.AddComponent<PrefabIdentifier>();
            prefabId.ClassId = "CompressorFab";
            prefabId.name = "Compressor";

            prefab.AddComponent<TechTag>().type = TechType;

            fabModel.transform.localPosition = new Vector3(fabModel.transform.localPosition.x, fabModel.transform.localPosition.y - 0.8f, fabModel.transform.localPosition.z);
            fabLight.transform.localPosition = new Vector3(fabLight.transform.localPosition.x, fabLight.transform.localPosition.y - 0.8f, fabLight.transform.localPosition.z);

            SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            global::Fabricator fabricator = prefab.GetComponent<global::Fabricator>();
            fabricator.craftTree = TreeTypeID;
            fabricator.handOverText = "Use Compressor";

            fabricator.GetComponent<GhostCrafter>().SetInstanceField("powerRelay", new PowerRelay());

            Constructable constructible = prefab.AddComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnWall = true;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = TechType; // This was necessary to correctly associate the recipe at building time
            constructible.model = fabModel;

            return prefab;
        }
    }

    public static class Extensions
    {
        public static List<T> Repeat<T>(this T obj, int c)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < c; i++) list.Add(obj);
            return list;
        }
    }
}
