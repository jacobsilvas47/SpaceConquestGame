using System.Collections.Generic;
using UnityEngine;

public class MissionConsolePanelUI : MonoBehaviour
{
    public enum FilterMode
    {
        All,
        Attack,
        Expedition
    }

    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ConsoleLogRowUI rowPrefab;

    private readonly List<ConsoleLogRowUI> rows = new();
    private FilterMode currentFilter = FilterMode.All;

    private void OnEnable()
    {
        Rebuild();
    }

    public void ShowAll()
    {
        currentFilter = FilterMode.All;
        Rebuild();
    }

    public void ShowAttack()
    {
        currentFilter = FilterMode.Attack;
        Rebuild();
    }

    public void ShowExpedition()
    {
        currentFilter = FilterMode.Expedition;
        Rebuild();
    }

    public void Rebuild()
    {
        ClearRows();

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            Debug.LogWarning("[MissionConsole] Missing GameState.");
            return;
        }

        GameState state = gameStateHolder.state;

        if (state.missionReports == null)
        {
            Debug.LogWarning("[MissionConsole] missionReports is null.");
            return;
        }

        Debug.Log($"[MissionConsole] missionReports count = {state.missionReports.Count}");

        for (int i = state.missionReports.Count - 1; i >= 0; i--)
        {
            MissionReport report = state.missionReports[i];
            if (report == null) continue;
            if (!MatchesFilter(report)) continue;

            ConsoleLogRowUI row = Instantiate(rowPrefab, contentRoot);
            row.Bind(report);
            rows.Add(row);

            Debug.Log($"[MissionConsole] Added row for {report.title}");
        }
    }

    private bool MatchesFilter(MissionReport report)
    {
        switch (currentFilter)
        {
            case FilterMode.Attack:
                return report.missionType == MissionType.Attack;

            case FilterMode.Expedition:
                return report.missionType == MissionType.Expedition;

            default:
                return true;
        }
    }

    private void ClearRows()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null)
                Destroy(rows[i].gameObject);
        }

        rows.Clear();
    }
}