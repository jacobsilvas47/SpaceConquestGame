using System;

[Serializable]
public class InventoryStack
{
    public ItemId id;
    public ItemRarity rarity;
    public int qty;

    public InventoryStack(ItemId id, ItemRarity rarity, int qty)
    {
        this.id = id;
        this.rarity = rarity;
        this.qty = qty;
    }
}