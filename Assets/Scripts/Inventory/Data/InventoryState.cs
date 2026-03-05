using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryState
{
    public List<InventoryStack> stacks = new List<InventoryStack>();

    public int GetQty(ItemId id)
    {
        var s = stacks.Find(x => x.id == id);
        return s == null ? 0 : s.qty;
    }

    public void Add(ItemId id, int amount)
    {
        if (amount <= 0) return;

        var s = stacks.Find(x => x.id == id);
        if (s == null) stacks.Add(new InventoryStack(id, amount));
        else s.qty += amount;
    }
}
