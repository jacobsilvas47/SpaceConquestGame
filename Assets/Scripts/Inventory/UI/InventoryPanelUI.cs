using UnityEngine;
using TMPro;

public class InventoryPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;   // InventoryPanel
    [SerializeField] private Transform contentRoot;  // ScrollView/Viewport/Content
    [SerializeField] private GameObject rowPrefab;   // InventoryRowPrefab

    public void Open()
    {
        if (panelRoot) panelRoot.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return;
        if (contentRoot == null || rowPrefab == null) return;

        // Clear old
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        var inv = gameStateHolder.state.inventory;

        // Empty message (optional): show nothing if empty
        for (int i = 0; i < inv.stacks.Count; i++)
        {
            var s = inv.stacks[i];
            var go = Instantiate(rowPrefab, contentRoot);
            var row = go.GetComponent<InventoryRowUI>();
            if (row != null) row.Bind(s.id.ToString(), s.qty);
        }
    }
}