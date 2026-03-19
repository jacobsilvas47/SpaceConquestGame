using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActiveMissionsPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private MissionRowUI rowPrefab;

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.25f;

    private readonly List<MissionRowUI> rows = new();
    private float nextRefreshTime;

    private void Start()
    {
        if (gameStateHolder == null)
            gameStateHolder = FindFirstObjectByType<GameStateHolder>();

        Rebuild();
    }

    private void OnEnable()
    {
        Rebuild();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime) return;
        nextRefreshTime = Time.unscaledTime + refreshInterval;

        Rebuild();
    }

    public void Rebuild()
    {
        ClearRows();

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            SetStatus("GameState not found.", Color.red);
            return;
        }

        if (contentRoot == null || rowPrefab == null)
        {
            SetStatus("Mission UI references missing.", Color.red);
            return;
        }

        GameState state = gameStateHolder.state;
        if (state.missions == null || state.missions.Count == 0)
        {
            SetStatus("No missions.", Color.white);
            return;
        }

        int shown = 0;

        for (int i = 0; i < state.missions.Count; i++)
        {
            Mission mission = state.missions[i];
            if (mission == null) continue;

            if (mission.status == MissionStatus.Completed || mission.status == MissionStatus.Failed)
                continue;

            string title = GetMissionTitle(mission);
            string target = MissionUIUtility.GetTargetDisplayName(state, mission);
            string status = GetStatusText(mission);
            string timer = GetTimerText(state, mission);

            MissionRowUI row = Instantiate(rowPrefab, contentRoot);
            row.Bind(title, target, status, timer);
            rows.Add(row);
            shown++;
        }

        SetStatus(shown == 0 ? "No active missions." : $"Active missions: {shown}", Color.white);
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

    private string GetMissionTitle(Mission mission)
    {
        switch (mission.missionType)
        {
            case MissionType.Attack:
                return "Attack";
            case MissionType.Expedition:
                return "Expedition";
            case MissionType.Transport:
                return "Transport";
            case MissionType.Deploy:
                return "Deploy";
            case MissionType.Spy:
                return "Spy";
            case MissionType.Harvest:
                return "Harvest";
            default:
                return mission.missionType.ToString();
        }
    }

    private string GetStatusText(Mission mission)
    {
        if (mission == null) return "Unknown";

        switch (mission.status)
        {
            case MissionStatus.EnRoute:
                return "En Route";
            case MissionStatus.Arrived:
                return "Arrived";
            case MissionStatus.Returning:
                return "Returning";
            case MissionStatus.Completed:
                return "Completed";
            case MissionStatus.Failed:
                return "Failed";
            default:
                return mission.status.ToString();
        }
    }

    private string GetTimerText(GameState state, Mission mission)
    {
        if (state == null || mission == null)
            return "--:--";

        double now = state.gameTime;
        double remaining = 0;

        switch (mission.status)
        {
            case MissionStatus.EnRoute:
                remaining = mission.arriveTime - now;
                break;

            case MissionStatus.Arrived:
                remaining = 0;
                break;

            case MissionStatus.Returning:
                remaining = mission.returnTime - now;
                break;

            default:
                remaining = 0;
                break;
        }

        if (remaining < 0)
            remaining = 0;

        return FormatTime(remaining);
    }

    private string FormatTime(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt((float)seconds));
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }

    private void SetStatus(string message, Color color)
    {
        if (statusText == null) return;

        statusText.text = message;
        statusText.color = color;
    }
}