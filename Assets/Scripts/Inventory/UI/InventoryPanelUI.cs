using UnityEngine;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject rowPrefab;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return;
        if (contentRoot == null || rowPrefab == null) return;

        ClearRows();

        var inventory = gameStateHolder.state.inventory;
        if (inventory == null || inventory.stacks == null) return;

        foreach (var stack in inventory.stacks)
        {
            if (stack == null) continue;
            if (stack.qty <= 0) continue;

            GameObject rowObj = Instantiate(rowPrefab, contentRoot);
            var rowUI = rowObj.GetComponent<InventoryRowUI>();

            if (rowUI != null)
                rowUI.Bind(stack);
        }
    }

    private void ClearRows()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }
    }
}