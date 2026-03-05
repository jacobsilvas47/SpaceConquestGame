using System;
using System.Collections.Generic;

[Serializable]
public class ExpeditionLogEntry
{
    public double timestamp;
    public string originPlanetId;
    public string missionId;

    public int metalGained;
    public int crystalGained;
    public int gasGained;

    public List<ItemStack> itemsGained = new List<ItemStack>();

    public string summaryText;
}

[Serializable]
public class ItemStack
{
    public string itemId;
    public int count;

    public ItemStack() { }  // Unity-friendly

    public ItemStack(string itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }
}