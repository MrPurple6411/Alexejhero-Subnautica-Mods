namespace ConfigurableDrillableCount;
using UnityEngine;

public class CDC_Config : MonoBehaviour
{
    public static int Min = 1;
    public static int Max = 3;

    public void Start()
    {
        UpdateNumbers();
    }

    public void UpdateNumbers()
    {
        Drillable drillable = gameObject.EnsureComponent<Drillable>();
        drillable.minResourcesToSpawn = Min;
        drillable.maxResourcesToSpawn = Max;
    }
}