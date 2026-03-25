using System.Collections.Generic;
using UnityEngine;

public static class ShipDatabase
{
    private static readonly Dictionary<ShipType, ShipData> dataByType = new()
    {
        {
            ShipType.Probe,
            new ShipData(
                "Probe",
                ShipTier.Light,
                ShipRole.Logistics,
                15,
                0,
                1,
                18,
                50,
                10,
                300,
                150,
                50,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 1 },
                    new Requirement { type = RequirementType.EngineTech, level = 1 }
                }
            )
        },
        {
            ShipType.SmallCargo,
            new ShipData(
                "Small Cargo",
                ShipTier.Light,
                ShipRole.Logistics,
                120,
                8,
                8,
                8,
                500,
                45,
                2000,
                1000,
                500,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 1 },
                    new Requirement { type = RequirementType.EngineTech, level = 3 }
                }
            )
        },
        {
            ShipType.LargeCargo,
            new ShipData(
                "Large Cargo",
                ShipTier.Light,
                ShipRole.Logistics,
                280,
                12,
                20,
                5,
                2000,
                90,
                6000,
                3500,
                1200,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 2 },
                    new Requirement { type = RequirementType.EngineTech, level = 6 }
                }
            )
        },
        {
            ShipType.Freighter,
            new ShipData(
                "Freighter",
                ShipTier.Medium,
                ShipRole.Logistics,
                650,
                20,
                45,
                3,
                8000,
                180,
                18000,
                12000,
                5000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 3 },
                    new Requirement { type = RequirementType.EngineTech, level = 12 },
                    new Requirement { type = RequirementType.ArmorTech, level = 8 }
                }
            )
        },
        {
            ShipType.BasicFighter,
            new ShipData(
                "Basic Fighter",
                ShipTier.Light,
                ShipRole.Combat,
                100,
                35,
                10,
                10,
                50,
                35,
                2500,
                1200,
                400,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 1 },
                    new Requirement { type = RequirementType.LaserTech, level = 5 }
                }
            )
        },
        {
            ShipType.Interceptor,
            new ShipData(
                "Interceptor",
                ShipTier.Light,
                ShipRole.Combat,
                130,
                45,
                18,
                14,
                40,
                50,
                4000,
                2200,
                900,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 2 },
                    new Requirement { type = RequirementType.LaserTech, level = 10 },
                    new Requirement { type = RequirementType.EngineTech, level = 8 }
                }
            )
        },
        {
            ShipType.LightFrigate,
            new ShipData(
                "Light Frigate",
                ShipTier.Medium,
                ShipRole.Combat,
                220,
                65,
                30,
                8,
                250,
                75,
                7000,
                3500,
                1500,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 3 },
                    new Requirement { type = RequirementType.LaserTech, level = 18 },
                    new Requirement { type = RequirementType.ArmorTech, level = 12 }
                }
            )
        },
        {
            ShipType.AssaultFighter,
            new ShipData(
                "Assault Fighter",
                ShipTier.Medium,
                ShipRole.Combat,
                260,
                90,
                45,
                7,
                100,
                100,
                10000,
                6000,
                2500,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 4 },
                    new Requirement { type = RequirementType.LaserTech, level = 28 },
                    new Requirement { type = RequirementType.EngineTech, level = 18 }
                }
            )
        },
        {
            ShipType.SiegeFrigate,
            new ShipData(
                "Siege Frigate",
                ShipTier.Heavy,
                ShipRole.Combat,
                500,
                160,
                90,
                5,
                300,
                180,
                22000,
                14000,
                7000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 4 },
                    new Requirement { type = RequirementType.LaserTech, level = 35 },
                    new Requirement { type = RequirementType.ArmorTech, level = 24 }
                }
            )
        },
        {
            ShipType.WarFrigate,
            new ShipData(
                "War Frigate",
                ShipTier.Heavy,
                ShipRole.Combat,
                700,
                220,
                120,
                5,
                500,
                240,
                32000,
                21000,
                11000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 5 },
                    new Requirement { type = RequirementType.LaserTech, level = 45 },
                    new Requirement { type = RequirementType.ArmorTech, level = 35 },
                    new Requirement { type = RequirementType.EngineTech, level = 24 }
                }
            )
        },
        {
            ShipType.Vanguard,
            new ShipData(
                "Vanguard",
                ShipTier.Heavy,
                ShipRole.Combat,
                1200,
                340,
                180,
                4,
                800,
                360,
                55000,
                40000,
                22000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 5 },
                    new Requirement { type = RequirementType.LaserTech, level = 55 },
                    new Requirement { type = RequirementType.ArmorTech, level = 45 },
                    new Requirement { type = RequirementType.EngineTech, level = 32 }
                }
            )
        },
        {
            ShipType.Titan,
            new ShipData(
                "Titan",
                ShipTier.Capital,
                ShipRole.Combat,
                1800,
                500,
                260,
                3,
                1200,
                480,
                85000,
                65000,
                35000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 6 },
                    new Requirement { type = RequirementType.LaserTech, level = 70 },
                    new Requirement { type = RequirementType.ArmorTech, level = 60 },
                    new Requirement { type = RequirementType.EngineTech, level = 45 }
                }
            )
        },
        {
            ShipType.Dreadnought,
            new ShipData(
                "Dreadnought",
                ShipTier.Capital,
                ShipRole.Combat,
                3000,
                800,
                420,
                2,
                2000,
                720,
                150000,
                110000,
                70000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 7 },
                    new Requirement { type = RequirementType.LaserTech, level = 80 },
                    new Requirement { type = RequirementType.ArmorTech, level = 75 },
                    new Requirement { type = RequirementType.EngineTech, level = 60 }
                }
            )
        },
        {
            ShipType.ColonyShip,
            new ShipData(
                "Colony Ship",
                ShipTier.Medium,
                ShipRole.Logistics,
                400,
                15,
                35,
                3,
                10000,
                300,
                18000,
                12000,
                8000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 2 },
                    new Requirement { type = RequirementType.EngineTech, level = 10 },
                    new Requirement { type = RequirementType.ArmorTech, level = 6 }
                }
            )
        },
        {
            ShipType.Salvager,
            new ShipData(
                "Salvager",
                ShipTier.Light,
                ShipRole.Logistics,
                220,
                5,
                20,
                4,
                5000,
                150,
                9000,
                5000,
                2500,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 2 },
                    new Requirement { type = RequirementType.EngineTech, level = 8 }
                }
            )
        },
        {
            ShipType.ShieldShip,
            new ShipData(
                "Shield Ship",
                ShipTier.Heavy,
                ShipRole.Support,
                900,
                40,
                300,
                3,
                200,
                260,
                30000,
                26000,
                12000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 3 },
                    new Requirement { type = RequirementType.ArmorTech, level = 22 },
                    new Requirement { type = RequirementType.EngineTech, level = 14 }
                }
            )
        },
        {
            ShipType.Bomber,
            new ShipData(
                "Bomber",
                ShipTier.Heavy,
                ShipRole.Combat,
                850,
                420,
                100,
                3,
                150,
                320,
                42000,
                30000,
                18000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.OrbitalShipworks, level = 4 },
                    new Requirement { type = RequirementType.LaserTech, level = 40 },
                    new Requirement { type = RequirementType.EngineTech, level = 20 }
                }
            )
        }
    };

    public static ShipData Get(ShipType type)
    {
        if (type == ShipType.None)
            return null;

        if (dataByType.TryGetValue(type, out var data))
            return data;

        Debug.LogWarning($"ShipDatabase: No data found for {type}");
        return null;
    }

    public static int CargoCapacity(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.cargoCapacity : 0;
    }

    public static int BuildTimeSeconds(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.buildTimeSeconds : 0;
    }

    public static int MetalCost(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.metalCost : 0;
    }

    public static int CrystalCost(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.crystalCost : 0;
    }

    public static int GasCost(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.gasCost : 0;
    }

    public static string DisplayName(ShipType type)
    {
        var data = Get(type);
        return data != null ? data.displayName : type.ToString();
    }
}