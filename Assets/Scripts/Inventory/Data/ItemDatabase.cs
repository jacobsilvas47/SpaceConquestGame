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

    public static string GetCategory(ItemId id)
    {
        switch (id)
        {
            case ItemId.LaserBlaster:
            case ItemId.PlasmaRifle:
            case ItemId.IonCannon:
                return "Weapon";

            case ItemId.FighterBlueprint:
            case ItemId.CargoUpgradeBlueprint:
            case ItemId.ProbeScannerBlueprint:
                return "Blueprint";

            case ItemId.AncientRelic:
            case ItemId.AlienCore:
            case ItemId.StarMapFragment:
                return "Relic";

            default:
                return "Unknown";
        }
    }
}