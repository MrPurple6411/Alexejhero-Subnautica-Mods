namespace PickupableStorageEnhanced;
using Nautilus.Utility;
using UnityEngine;

public static class PFC_Config
{
    public static bool Enable
    {
        get
        {
            return PlayerPrefsExtra.GetBool("pfcEnable", true);
        }
        set
        {
            PlayerPrefsExtra.SetBool("pfcEnable", value);
            PlayerPrefs.Save();
        }
    }
    public static string AllowMMB
    {
        get
        {
            return PlayerPrefs.GetString("pfcMMB", "Yes");
        }
        set
        {
            PlayerPrefs.SetString("pfcMMB", value);
            PlayerPrefs.Save();
        }
    }

    public static string[] AllowMMBOptions =
    {
        "Yes",
        "Only in player inventory",
        "No",
    };
}
