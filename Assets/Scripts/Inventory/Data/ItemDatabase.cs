using UnityEngine;
public static class ItemDatabase
{
    public static string GetName(ItemId id)
    {
        switch (id)
        {
            case ItemId.LaserBlaster: return "Laser Blaster";
            case ItemId.PlasmaRifle: return "Plasma Rifle";
            case ItemId.IonCannon: return "Ion Cannon";

            case ItemId.FighterBlueprint: return "Fighter Blueprint";
            case ItemId.CargoUpgradeBlueprint: return "Cargo Upgrade Blueprint";
            case ItemId.ProbeScannerBlueprint: return "Probe Scanner Blueprint";

            case ItemId.AncientRelic: return "Ancient Relic";
            case ItemId.AlienCore: return "Alien Core";
            case ItemId.StarMapFragment: return "Star Map Fragment";

            default: return id.ToString();
        }
    }

    public static string GetDisplayName(ItemId id, ItemRarity rarity)
    {
        string baseName = GetName(id);
        string prefix = GetRarityPrefix(rarity);
        return $"{prefix} {baseName}";
    }

    public static ItemCategory GetCategory(ItemId id)
    {
        switch (id)
        {
            case ItemId.LaserBlaster:
            case ItemId.PlasmaRifle:
            case ItemId.IonCannon:
                return ItemCategory.Weapon;

            case ItemId.FighterBlueprint:
            case ItemId.CargoUpgradeBlueprint:
            case ItemId.ProbeScannerBlueprint:
                return ItemCategory.Blueprint;

            case ItemId.AncientRelic:
            case ItemId.AlienCore:
            case ItemId.StarMapFragment:
                return ItemCategory.Relic;

            default:
                return ItemCategory.Treasure;
        }
    }

    public static string GetRarityPrefix(ItemRarity rarity)
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

    public static ItemUse GetUses(ItemId id)
    {
        switch (id)
        {
            case ItemId.LaserBlaster:
            case ItemId.PlasmaRifle:
            case ItemId.IonCannon:
                return ItemUse.Sell | ItemUse.Craft;

            case ItemId.FighterBlueprint:
            case ItemId.CargoUpgradeBlueprint:
            case ItemId.ProbeScannerBlueprint:
                return ItemUse.Unlock | ItemUse.Salvage;

            case ItemId.AncientRelic:
                return ItemUse.Sell | ItemUse.Research;

            case ItemId.AlienCore:
                return ItemUse.Research | ItemUse.Craft;

            case ItemId.StarMapFragment:
                return ItemUse.Research | ItemUse.Sell;

            default:
                return ItemUse.None;
        }
    }

    public static int GetBaseValue(ItemId id)
    {
        switch (id)
        {
            case ItemId.LaserBlaster: return 100;
            case ItemId.PlasmaRifle: return 150;
            case ItemId.IonCannon: return 250;

            case ItemId.FighterBlueprint: return 200;
            case ItemId.CargoUpgradeBlueprint: return 180;
            case ItemId.ProbeScannerBlueprint: return 160;

            case ItemId.AncientRelic: return 300;
            case ItemId.AlienCore: return 500;
            case ItemId.StarMapFragment: return 220;

            default: return 0;
        }
    }

    public static float GetRarityMultiplier(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return 1f;
            case ItemRarity.Uncommon: return 1.5f;
            case ItemRarity.Rare: return 2.25f;
            case ItemRarity.Epic: return 3.5f;
            case ItemRarity.Legendary: return 5f;
            case ItemRarity.Alien: return 8f;
            default: return 1f;
        }
    }

    public static int GetValue(ItemId id, ItemRarity rarity)
    {
        return Mathf.RoundToInt(GetBaseValue(id) * GetRarityMultiplier(rarity));
    }
}