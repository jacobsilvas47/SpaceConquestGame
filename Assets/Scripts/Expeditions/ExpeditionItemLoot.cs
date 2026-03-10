using System;
using UnityEngine;

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

        public static ItemRarity RollRarity(System.Random rng, float quality01)
    {
        quality01 = Mathf.Clamp01(quality01);

        int commonWeight = Mathf.RoundToInt(Mathf.Lerp(550, 350, quality01));
        int uncommonWeight = Mathf.RoundToInt(Mathf.Lerp(250, 280, quality01));
        int rareWeight = Mathf.RoundToInt(Mathf.Lerp(120, 200, quality01));
        int epicWeight = Mathf.RoundToInt(Mathf.Lerp(50, 110, quality01));
        int legendaryWeight = Mathf.RoundToInt(Mathf.Lerp(25, 50, quality01));
        int alienWeight = Mathf.RoundToInt(Mathf.Lerp(5, 10, quality01));

        int totalWeight = commonWeight + uncommonWeight + rareWeight + epicWeight + legendaryWeight + alienWeight;
        int roll = rng.Next(0, totalWeight);

        if (roll < commonWeight) return ItemRarity.Common;
        roll -= commonWeight;

        if (roll < uncommonWeight) return ItemRarity.Uncommon;
        roll -= uncommonWeight;

        if (roll < rareWeight) return ItemRarity.Rare;
        roll -= rareWeight;

        if (roll < epicWeight) return ItemRarity.Epic;
        roll -= epicWeight;

        if (roll < legendaryWeight) return ItemRarity.Legendary;
        return ItemRarity.Alien;
    }
}