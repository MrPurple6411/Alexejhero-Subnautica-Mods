namespace PickupableStorageEnhanced;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public static class InventoryOpener
{
    public static InventoryItem LastOpened;
    public static uGUI_ItemsContainer InventoryUGUI;
    public static bool DontEnable;

    public static void OnMiddleClick(InventoryItem item)
    {
        Vector2int cursorPosition = GetCursorPosition();

        DontEnable = true;
        Player.main.GetPDA().Close();
        DontEnable = false;

        StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
        container.Open();
        container.onUse.Invoke();

        if (PlayerInventoryContains(item))
        {
            if (LastOpened != null)
            {
                LastOpened.isEnabled = true;
                GetIconForItem(LastOpened)?.SetChroma(1f);
            }
            item.isEnabled = false;
            GetIconForItem(item)?.SetChroma(0f);
            LastOpened = item;
        }

        GameObject.FindObjectOfType<GameInput>().StartCoroutine(ResetCursor(cursorPosition));
    }
    public static bool Condition(InventoryItem item)
    {
        return PFC_Config.Enable && CanOpen(item);
    }
    public static bool CanOpen(InventoryItem item)
    {
        return PFC_Config.AllowMMB == "Yes" || (PFC_Config.AllowMMB == "Only in player inventory" && PlayerInventoryContains(item));
    }
    public static bool PlayerInventoryContains(InventoryItem item)
    {
        IList<InventoryItem> matchingItems = Inventory.main.container.GetItems(item.item.GetTechType());
        return matchingItems != null && matchingItems.Contains(item);
    }
    public static uGUI_ItemIcon GetIconForItem(InventoryItem item)
    {
        Dictionary<InventoryItem, uGUI_ItemIcon> items = typeof(uGUI_ItemsContainer).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InventoryUGUI) as Dictionary<InventoryItem, uGUI_ItemIcon>;
        return items[item];
    }
    #region Mouse Position
    public static IEnumerator ResetCursor(Vector2int position)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        SetCursorPosition(position);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }
    public static Vector2int GetCursorPosition()
    {
        GetCursorPos(out Point point);
        return new Vector2int(point.X, point.Y);
    }
    public static void SetCursorPosition(Vector2int position)
    {
        SetCursorPos(position.x, position.y);
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    #endregion
}
