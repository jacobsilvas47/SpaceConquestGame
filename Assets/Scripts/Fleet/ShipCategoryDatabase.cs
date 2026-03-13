public static class ShipCategoryDatabase
{
    public static ShipCategory GetCategory(ShipType type)
    {
        switch (type)
        {
            case ShipType.Probe:
            case ShipType.Salvager:
                return ShipCategory.ScoutsAndUtility;

            case ShipType.SmallCargo:
            case ShipType.LargeCargo:
            case ShipType.Freighter:
                return ShipCategory.CargoAndLogistics;

            case ShipType.ColonyShip:
                return ShipCategory.Colonization;

            case ShipType.BasicFighter:
            case ShipType.Interceptor:
            case ShipType.AssaultFighter:
                return ShipCategory.LightCombat;

            case ShipType.LightFrigate:
            case ShipType.SiegeFrigate:
            case ShipType.WarFrigate:
                return ShipCategory.Frigates;

            case ShipType.ShieldShip:
            case ShipType.Bomber:
                return ShipCategory.SpecialistCombat;

            case ShipType.Vanguard:
            case ShipType.Titan:
            case ShipType.Dreadnought:
                return ShipCategory.CapitalShips;

            default:
                return ShipCategory.All;
        }
    }
}