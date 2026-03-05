using System.Text;
using TMPro;
using UnityEngine;

public class ConsolePageUI : MonoBehaviour
{
    [Header("Refs")]
    public GameStateHolder gameStateHolder;

    [Header("UI")]
    public Transform contentParent;       // LogScrollView/Viewport/Content
    public GameObject logRowPrefab;       // your LogRow prefab

    public GameObject consolePage;        // ConsolePage root
    public GameObject returnPage;         // ExpeditionsPage (or MainPage)

    public void Open()
    {
        if (consolePage) consolePage.SetActive(true);
        if (returnPage) returnPage.SetActive(false);
        Refresh();
    }

    public void Close()
    {
        if (consolePage) consolePage.SetActive(false);
        if (returnPage) returnPage.SetActive(true);
    }

    public void Refresh()
    {
        if (contentParent == null || logRowPrefab == null) return;
        if (gameStateHolder == null || gameStateHolder.state == null) return;

        // Clear existing rows
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        var log = gameStateHolder.state.expeditionLog;

        // Show newest first (optional)
        for (int i = log.Count - 1; i >= 0; i--)
        {
            var entry = log[i];
            var go = Instantiate(logRowPrefab, contentParent);

            var row = go.GetComponent<ConsoleLogRowUI>();
            if (row != null)
                row.Bind(entry);
        }
    }
}