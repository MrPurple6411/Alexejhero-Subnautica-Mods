namespace PickupableStorageEnhanced;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public static class InventoryOpener
{
    public static InventoryItem LastOpened;
    public static StorageContainer LastOpenedContainer;

    public static uGUI_ItemsContainer InventoryUGUI;

    public static void OnMiddleClick(InventoryItem item)
    {
        Player.main.StartCoroutine(OpenContainer(item));
    }

    public static IEnumerator OpenContainer(InventoryItem item)
    {
        Vector2int cursorPosition = GetCursorPosition();

        StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;

        PDA pda = Player.main.GetPDA();
        pda.Close();

        Inventory.main.SetUsedStorage(container.container, false);
        yield return new WaitUntil(() => pda.Open(PDATab.Inventory, container.transform, new PDA.OnClose(container.OnClosePDA)));

        container.open = true;

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
            LastOpenedContainer = container;
        }

        Object.FindObjectOfType<GameInput>().StartCoroutine(ResetCursor(cursorPosition));
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
        return InventoryUGUI.items[item];
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
