using UnityEngine;
using TMPro;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI qtyText;

    public void Bind(string itemName, int qty)
    {
        if (itemNameText) itemNameText.text = itemName;
        if (qtyText) qtyText.text = qty.ToString();
    }
}