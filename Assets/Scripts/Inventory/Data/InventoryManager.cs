using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryStack> items = new List<InventoryStack>();

    public void AddItem(ItemId id, ItemRarity rarity, int amount)
    {
        if (amount <= 0)
            return;

        InventoryStack existing = items.Find(x => x.id == id && x.rarity == rarity);

        if (existing != null)
        {
            existing.qty += amount;
        }
        else
        {
            items.Add(new InventoryStack(id, rarity, amount));
        }

        Debug.Log($"[INVENTORY] Added {amount}x {ItemDatabase.GetDisplayName(id, rarity)}");
    }

    public bool RemoveItem(ItemId id, ItemRarity rarity, int amount)
    {
        if (amount <= 0)
            return false;

        InventoryStack existing = items.Find(x => x.id == id && x.rarity == rarity);

        if (existing == null || existing.qty < amount)
            return false;

        existing.qty -= amount;

        if (existing.qty <= 0)
            items.Remove(existing);

        return true;
    }

    public int GetQuantity(ItemId id, ItemRarity rarity)
    {
        InventoryStack existing = items.Find(x => x.id == id && x.rarity == rarity);
        return existing != null ? existing.qty : 0;
    }
}