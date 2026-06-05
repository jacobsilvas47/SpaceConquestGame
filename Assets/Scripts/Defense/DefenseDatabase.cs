using System.Collections.Generic;

public static class DefenseDatabase
{
    private static readonly Dictionary<DefenseType, DefenseData> dataByType = new()
    {
        {
            DefenseType.LaserTurret,
            new DefenseData(
                DefenseType.LaserTurret,
                "Laser Turret",
                500, 150, 0,
                40, 120,
                0,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.Engineering, level = 1 },
                    new Requirement { type = RequirementType.WeaponSystems, level = 1 }
                }
            )
        },
        {
            DefenseType.MissileBattery,
            new DefenseData(
                DefenseType.MissileBattery,
                "Missile Battery",
                1200, 400, 0,
                120, 180,
                0,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.Engineering, level = 2 },
                    new Requirement { type = RequirementType.WeaponSystems, level = 2 }
                }
            )
        },
        {
            DefenseType.ShieldGenerator,
            new DefenseData(
                DefenseType.ShieldGenerator,
                "Shield Generator",
                2500, 1500, 0,
                0, 250,
                1000,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.Engineering, level = 3 },
                    new Requirement { type = RequirementType.DefenseSystems, level = 2 }
                }
            )
        },
        {
            DefenseType.PlanetaryCannon,
            new DefenseData(
                DefenseType.PlanetaryCannon,
                "Planetary Cannon",
                8000, 5000, 1000,
                450, 700,
                0,
                new List<Requirement>
                {
                    new Requirement { type = RequirementType.Engineering, level = 5 },
                    new Requirement { type = RequirementType.WeaponSystems, level = 5 },
                    new Requirement { type = RequirementType.DefenseSystems, level = 4 }
                }
            )
        }
    };

    public static DefenseData Get(DefenseType type)
    {
        return dataByType.TryGetValue(type, out var data) ? data : null;
    }

    public static IEnumerable<DefenseData> GetAll()
    {
        return dataByType.Values;
    }
}