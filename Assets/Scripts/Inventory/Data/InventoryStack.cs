using System;
using UnityEngine;

[Serializable]
public class InventoryStack
{
    public ItemId id;
    public int qty;

    public InventoryStack(ItemId id, int qty)
    {
        this.id = id;
        this.qty = qty;
    }
}