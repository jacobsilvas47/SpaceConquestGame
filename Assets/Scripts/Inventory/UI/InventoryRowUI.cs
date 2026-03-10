using TMPro;
using UnityEngine;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI qtyText;
    [SerializeField] private TextMeshProUGUI categoryText;

    public void Bind(InventoryStack stack)
    {
        if (stack == null) return;

        if (nameText)
    {
        nameText.text = ItemDatabase.GetDisplayName(stack.id, stack.rarity);
        nameText.color = ItemRarityUtility.GetColor(stack.rarity);
    }
        if (qtyText) qtyText.text = $"x{stack.qty}";
        if (categoryText) categoryText.text = ItemDatabase.GetCategory(stack.id).ToString();
    }
}