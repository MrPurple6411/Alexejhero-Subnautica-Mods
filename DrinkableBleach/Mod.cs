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
        public static void Patch()
        {
            HarmonyHelper.Patch();

            Logger.Log("Patched successfully!");

            LanguageHandler.SetLanguageLine("Tooltip_Bleach", "NaClO. Sodium hypochlorite bleach. Sanitizing applications. Do not drink.");

            Logger.Log("Updated Bleach tooltip");
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
                if (techType == TechType.Bleach)
                {
                    __result.AddComponent<DrinkableBleach>();

                    Logger.Log($"Added components to bleach item!");
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