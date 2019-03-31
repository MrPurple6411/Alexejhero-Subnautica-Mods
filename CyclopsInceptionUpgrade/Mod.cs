using AlexejheroYTB.Common;
using Harmony;
using MoreCyclopsUpgrades.CyclopsUpgrades;
using MoreCyclopsUpgrades.Managers;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AlexejheroYTB.CyclopsInceptionUpgrade
{
    public static class Mod
    {
        public static string assembly = Assembly.GetCallingAssembly().GetName().Name;

        public static void Patch()
        {
            TechType techType = TechTypeHandler.AddTechType("CyclopsInceptionModule", "Cyclops Inception Module", "Uses precursor technology to shrink any other cyclops near the docking hatch, allowing them to dock.");
            CraftDataHandler.SetEquipmentType(techType, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, techType);
            UpgradeManager.RegisterReusableHandlerCreator(() => new InceptionUpgrade(techType));

            HarmonyHelper.Patch();
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(VehicleDockingBay))]
        [HarmonyPatch("OnTriggerEnter")]
        public static class VehicleDockingBay_OnTriggerEnter
        {
            [HarmonyPrefix]
            public static bool Prefix(VehicleDockingBay __instance, Collider other)
            {
                SubRoot cyclops = UWE.Utils.GetComponentInHierarchy<SubRoot>(other.gameObject);
                if (cyclops == null || InceptionManager.DockedCyclopses.Contains(cyclops) || InceptionManager.GetRecentlyUndocked(cyclops))
                {
                    return;
                }
                if (this.GetDockedVehicle())
                {
                    return;
                }
                if (GameModeUtils.RequiresPower() && !this.powered)
                {
                    return;
                }
                if (this.interpolatingVehicle != null)
                {
                    return;
                }
                this.timeDockingStarted = Time.time;
                this.interpolatingVehicle = componentInHierarchy;
                this.startPosition = this.interpolatingVehicle.transform.position;
                this.startRotation = this.interpolatingVehicle.transform.rotation;
            }
        }

        /*
        [HarmonyPatch(typeof(VehicleDockingBay))]
        [HarmonyPatch("LaunchbayAreaEnter")]
        public static class VehicleDockingBay_LaunchbayAreaEnter
        {
            [HarmonyPrefix]
            public static bool Prefix(VehicleDockingBay __instance, GameObject nearby)
            {
                SubRoot cyclops = UWE.Utils.GetComponentInHierarchy<SubRoot>(nearby.gameObject);
                if (cyclops != null && !InceptionManager.DockedCyclopses.Contains(cyclops))
                {
                    if ((bool)__instance.GetInstanceField("powered"))
                    {
                        SubRoot main = __instance.GetSubRoot();
                        main.BroadcastMessage("OnLaunchBayOpening", SendMessageOptions.DontRequireReceiver);
                        main.BroadcastMessage("LockDoors", SendMessageOptions.DontRequireReceiver);
                        if ((bool)__instance.GetInstanceField("soundReset"))
                        {
                            if (__instance.bayDoorsOpenSFX != null)
                            {
                                __instance.bayDoorsOpenSFX.Play();
                            }
                            __instance.SetInstanceField("soundReset", false);
                            __instance.Invoke("SoundReset", 1f);
                        }
                    }
                    __instance.SetInstanceField("nearbyVehicle", null);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(VehicleDockingBay))]
        [HarmonyPatch("LaunchbayAreaExit")]
        public static class VehicleDockingBay_LaunchbayAreaExit
        {
            [HarmonyPrefix]
            public static bool Prefix(VehicleDockingBay __instance, GameObject nearby)
            {
                SubRoot subRoot = UWE.Utils.GetComponentInHierarchy<SubRoot>(nearby.gameObject);
                if (subRoot != null)
                {
                    __instance.GetSubRoot().BroadcastMessage("UnlockDoors", SendMessageOptions.DontRequireReceiver);
                    if ((bool)__instance.GetInstanceField("soundReset"))
                    {
                        if (__instance.bayDoorsCloseSFX != null)
                        {
                            __instance.bayDoorsCloseSFX.Play();
                        }
                        __instance.SetInstanceField("soundReset", false);
                        __instance.Invoke("SoundReset", 1f);
                    }
                    return false;
                }
                return true;
            }
        }
        */
    }

    public class InceptionManager : MonoBehaviour
    {
        public static readonly List<SubRoot> CyclopsesWithUpgrade = new List<SubRoot>();
        public static readonly List<SubRoot> DockedCyclopses = new List<SubRoot>();
        public static readonly Dictionary<SubRoot, float> RecentlyUndockedTime = new Dictionary<SubRoot, float>();

        public static bool GetRecentlyUndocked(SubRoot cyclops)
        {

        }
    }

    public class InceptionUpgrade : UpgradeHandler
    {
        public InceptionUpgrade(TechType techType) : base(techType)
        {
            IsAllowedToAdd += IsAllowedToAdd;
            IsAllowedToRemove += IsAllowedToRemove;
            OnClearUpgrades += OnClear;
            OnUpgradeCounted += OnCount;
            MaxCount = 1;
        }

        public void OnClear(SubRoot cyclops)
        {
            ErrorMessage.AddDebug("Removed");
            if (InceptionManager.CyclopsesWithUpgrade.Contains(cyclops))
                InceptionManager.CyclopsesWithUpgrade.Remove(cyclops);
        }

        public void OnCount(SubRoot cyclops, Equipment modules, string slot)
        {
            ErrorMessage.AddDebug("Added");
            if (!InceptionManager.CyclopsesWithUpgrade.Contains(cyclops))
                InceptionManager.CyclopsesWithUpgrade.Add(cyclops);
        }

        public bool AllowedToAdd(SubRoot cyclops, Pickupable item, bool verbose)
        {
            return true;
        }

        public bool AllowedToRemove(SubRoot cyclops, Pickupable item, bool verbose)
        {
            return true;
        }
    }
}