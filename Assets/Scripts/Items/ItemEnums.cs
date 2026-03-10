public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Alien
}

public enum ItemCategory
{
    Weapon,
    Blueprint,
    Relic,
    Research,
    Crafting,
    Treasure
}

[System.Flags]
public enum ItemUse
{
    None = 0,
    Sell = 1,
    Craft = 2,
    Research = 4,
    Equip = 8,
    Unlock = 16,
    Salvage = 32,
    Upgrade = 64,
    Trade = 128
}