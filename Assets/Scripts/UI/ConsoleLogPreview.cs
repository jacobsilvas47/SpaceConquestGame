using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

[ExecuteAlways]
public class ConsoleLogPreview : MonoBehaviour
{
    [Header("Prefab + Target")]
    public GameObject logRowPrefab;

    [Header("Preview")]
    public bool enablePreview = true;
    [Range(1, 30)] public int previewCount = 8;

    [TextArea(2, 6)]
    public string[] previewLines =
    {
        "Expedition Result: +16840 Metal, +5455 Crystal, +2590 Gas",
        "Expedition Result: Came up empty, no resources found.",
        "Expedition Result: Found abandoned ships: +1 Probes, +1 Small Cargo",
        "Expedition Result: JACKPOT, +22000 Metal, +12000 Crystal, +6000 Gas"
    };

    private void OnEnable()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }

    [ContextMenu("Refresh Preview")]
    public void Refresh()
    {
        if (!enablePreview) return;
        if (Application.isPlaying) return;
        if (logRowPrefab == null) return;

        // Clear current children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
#if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        // Spawn preview rows
        for (int i = 0; i < previewCount; i++)
        {
            var go =
#if UNITY_EDITOR
                (GameObject)PrefabUtility.InstantiatePrefab(logRowPrefab, transform);
#else
                Instantiate(logRowPrefab, transform);
#endif

            var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                string line = (previewLines != null && previewLines.Length > 0)
                    ? previewLines[i % previewLines.Length]
                    : $"Log line {i + 1}";

                // Add a little variety for testing wrap
                tmp.text = $"00:{(i * 7) % 60:00}  {line}";
            }

            go.name = $"PREVIEW_LogRow_{i:00}";
        }
    }
}