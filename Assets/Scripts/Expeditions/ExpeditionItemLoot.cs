using System;

public static class ExpeditionItemLoot
{
    public static bool TryRollItemReward(System.Random rng, out ItemId itemId, out int amount)
    {
        itemId = ItemId.None;
        amount = 0;

        if (rng == null) return false;

        // 20% chance to get an item
        int dropRoll = rng.Next(0, 100);
        if (dropRoll >= 20)
            return false;

        int itemRoll = rng.Next(0, 100);

        if (itemRoll < 25)
        {
            itemId = ItemId.AncientRelic;
            amount = 1;
        }
        else if (itemRoll < 45)
        {
            itemId = ItemId.AlienCore;
            amount = 1;
        }
        else if (itemRoll < 60)
        {
            itemId = ItemId.StarMapFragment;
            amount = rng.Next(1, 3); // 1-2
        }
        else if (itemRoll < 74)
        {
            itemId = ItemId.LaserBlaster;
            amount = 1;
        }
        else if (itemRoll < 84)
        {
            itemId = ItemId.PlasmaRifle;
            amount = 1;
        }
        else if (itemRoll < 90)
        {
            itemId = ItemId.IonCannon;
            amount = 1;
        }
        else if (itemRoll < 94)
        {
            itemId = ItemId.FighterBlueprint;
            amount = 1;
        }
        else if (itemRoll < 97)
        {
            itemId = ItemId.CargoUpgradeBlueprint;
            amount = 1;
        }
        else
        {
            itemId = ItemId.ProbeScannerBlueprint;
            amount = 1;
        }

        return true;
    }
}