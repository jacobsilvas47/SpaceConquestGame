using UnityEngine;

[CreateAssetMenu(menuName = "Space Conquest/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string baseName;

    [TextArea]
    public string description;

    [Header("Type")]
    public ItemCategory category;
    public ItemUse uses;

    [Header("Value")]
    public int baseSellValue;
    public int baseResearchValue;
    public int baseCraftValue;

    [Header("Visuals")]
    public Sprite icon;

    [Header("Rarity")]
    public ItemRarity rarity = ItemRarity.Common;

    [Header("Expedition Unlock")]
    public ExpeditionTier minimumExpeditionTier = ExpeditionTier.Tier1;
}