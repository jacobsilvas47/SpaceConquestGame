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
                40, 120
            )
        },
        {
            DefenseType.MissileBattery,
            new DefenseData(
                DefenseType.MissileBattery,
                "Missile Battery",
                1200, 400, 0,
                120, 180
            )
        },
        {
            DefenseType.ShieldGenerator,
            new DefenseData(
                DefenseType.ShieldGenerator,
                "Shield Generator",
                2500, 1500, 0,
                0, 250, 1000
            )
        },
        {
            DefenseType.PlanetaryCannon,
            new DefenseData(
                DefenseType.PlanetaryCannon,
                "Planetary Cannon",
                8000, 5000, 1000,
                450, 700
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