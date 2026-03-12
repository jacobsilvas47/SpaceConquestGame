using UnityEngine;

public static class PirateFleetFactory
{
    public static Fleet CreatePirateFleet(int playerPower)
    {
        Fleet pirates = new Fleet("pirates");

        // Make pirate strength scale roughly with player fleet power
        if (playerPower < 1000)
        {
            pirates.AddShips(ShipType.BasicFighter, Random.Range(2, 5));
            pirates.AddShips(ShipType.SmallCargo, Random.Range(0, 2));
        }
        else if (playerPower < 3000)
        {
            pirates.AddShips(ShipType.BasicFighter, Random.Range(4, 9));
            pirates.AddShips(ShipType.Interceptor, Random.Range(1, 4));
            pirates.AddShips(ShipType.SmallCargo, Random.Range(1, 3));
        }
        else if (playerPower < 7000)
        {
            pirates.AddShips(ShipType.BasicFighter, Random.Range(6, 12));
            pirates.AddShips(ShipType.Interceptor, Random.Range(3, 6));
            pirates.AddShips(ShipType.LightFrigate, Random.Range(1, 3));
            pirates.AddShips(ShipType.SmallCargo, Random.Range(1, 3));
        }
        else
        {
            pirates.AddShips(ShipType.AssaultFighter, Random.Range(3, 6));
            pirates.AddShips(ShipType.Interceptor, Random.Range(4, 8));
            pirates.AddShips(ShipType.LightFrigate, Random.Range(2, 4));
            pirates.AddShips(ShipType.SmallCargo, Random.Range(1, 4));
        }

        return pirates;
    }
}