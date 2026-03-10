using UnityEngine;

public static class ItemRarityUtility
{
    public static string GetPrefix(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return "Basic";
            case ItemRarity.Uncommon: return "Refined";
            case ItemRarity.Rare: return "Advanced";
            case ItemRarity.Epic: return "Elite";
            case ItemRarity.Legendary: return "Prototype";
            case ItemRarity.Alien: return "Alien";
            default: return "";
        }
    }

    public static Color GetColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return Color.white;
            case ItemRarity.Uncommon: return new Color(0.2f, 0.8f, 0.2f);
            case ItemRarity.Rare: return new Color(0.2f, 0.5f, 1f);
            case ItemRarity.Epic: return new Color(0.7f, 0.3f, 1f);
            case ItemRarity.Legendary: return new Color(1f, 0.6f, 0.1f);
            case ItemRarity.Alien: return new Color(0.2f, 1f, 0.9f);
            default: return Color.white;
        }
    }
}