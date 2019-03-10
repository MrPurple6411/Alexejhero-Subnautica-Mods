using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using System;
using System.Reflection;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.DrinkableBleach
{
    public static class QMod
    {
        public static string assembly = Assembly.GetExecutingAssembly().GetName().Name;

        public static void Patch()
        {
            try
            {
                HarmonyHelper.Patch();

                Logger.Log("Patched successfully!");

                LanguageHandler.SetLanguageLine("Tooltip_Bleach", "NaClO. Sodium hypochlorite bleach. Sanitizing applications.\n(If you cannot drink it, you need to craft a new one)");

                Logger.Log("Updated Bleach tooltip");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Initializing);
            }
        }
    }

    [DisallowMultipleComponent]
    public class DrinkableBleach : MonoBehaviour
    {
        public Eatable eatable;

        public void Start()
        {
            if (gameObject.GetComponent<Player>() != null) return;

            eatable = gameObject.AddComponent<Eatable>();
            eatable.decomposes = false;
            eatable.despawns = false;
            eatable.foodValue = -10;
            eatable.waterValue = 5;
        }

        public float Timer = 10f;

        public void Update()
        {
            if (gameObject.GetComponent<Player>() == null) return;

            if (Timer <= 0)
            {
                Destroy(this);
                return;
            }

            Player.main.OnTakeDamage(new DamageInfo() { damage = Time.deltaTime * 10, type = DamageType.Starve, dealer = gameObject, originalDamage = Time.deltaTime * 10, position = transform.position });
        }

        public void OnDrink()
        {
            Player.main.gameObject.AddComponent<DrinkableBleach>();
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(CraftData), "GetPrefabForTechType")]
        public static class CraftData_GetPrefabForTechType
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject __result, TechType techType)
            {
                try
                {
                    if (techType == TechType.Bleach)
                    {
                        __result.AddComponent<DrinkableBleach>();

                        Logger.Log($"Added components to bleach item!", QMod.assembly);
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                }
            }
        }

        [HarmonyPatch(typeof(Survival), "Eat")]
        public static class Survival_Eat
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject useObj)
            {
                if (useObj.GetComponent<DrinkableBleach>() is DrinkableBleach drinkableBleach)
                {
                    drinkableBleach.OnDrink();
                }
            }
        }
    }
}