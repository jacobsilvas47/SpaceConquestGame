using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryState
{
    public List<InventoryStack> stacks = new List<InventoryStack>();

    public int GetQty(ItemId id, ItemRarity rarity)
    {
        var s = stacks.Find(x => x.id == id && x.rarity == rarity);
        return s == null ? 0 : s.qty;
    }

    public void Add(ItemId id, ItemRarity rarity, int amount)
    {
        if (amount <= 0) return;

        var s = stacks.Find(x => x.id == id && x.rarity == rarity);

        if (s == null)
            stacks.Add(new InventoryStack(id, rarity, amount));
        else
            s.qty += amount;
    }
}