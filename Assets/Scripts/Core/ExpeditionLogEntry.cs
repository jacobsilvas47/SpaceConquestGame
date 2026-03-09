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

        public List<ExpeditionItemReward> itemsGained = new List<ExpeditionItemReward>();

        public string summaryText;
    }

    [Serializable]
    public class ExpeditionItemReward
    {
        public ItemId itemId;
        public int amount;

        public ExpeditionItemReward() { }

        public ExpeditionItemReward(ItemId itemId, int amount)
        {
            this.itemId = itemId;
            this.amount = amount;
        }
    }