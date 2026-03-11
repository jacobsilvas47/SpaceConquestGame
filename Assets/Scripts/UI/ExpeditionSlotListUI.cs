using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ExpeditionSlotsListUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI slotsText;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private ExpeditionSlotRowUI rowPrefab;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.25f;

    private float nextRefreshTime;
    private readonly List<ExpeditionSlotRowUI> rows = new();

    // Optional: let your existing ExpeditionPanelUI listen to selection
    public System.Action<string> onMissionSelected;

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime) return;
        nextRefreshTime = Time.unscaledTime + refreshInterval;

        Rebuild();
    }

    void OnEnable()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return;
        if (slotsContainer == null || rowPrefab == null) return;

        var state = gameStateHolder.state;

        int slots = System.Math.Max(1, state.expeditionSlotsUnlocked);

        // Grab “active” expeditions for this planet
        List<ExpeditionMission> active = state.missions
            .OfType<ExpeditionMission>()
            .Where(m => m != null && m.originPlanetId == planetId && state.gameTime < m.returnTime)
            .OrderBy(m => m.returnTime)
            .ToList();

        // Update slots header
        if (slotsText) slotsText.text = $"Expedition Slots: {active.Count}/{slots}";

        EnsureRowCount(slots);

        // Fill rows 1..slots
        for (int i = 0; i < slots; i++)
        {
            int slotIndex = i + 1;

            if (i >= active.Count)
            {
                rows[i].BindEmpty(slotIndex);
                continue;
            }

            ExpeditionMission m = active[i];
            string status;
            string timer;

            GetStatusAndTimer(state.gameTime, m, out status, out timer);

            rows[i].BindMission(slotIndex, m.missionId, status, timer, HandleRowSelected);
        }
    }

    private void HandleRowSelected(string missionId)
    {
        onMissionSelected?.Invoke(missionId);
    }

    private void EnsureRowCount(int count)
    {
        // Create more rows if needed
        while (rows.Count < count)
        {
            var row = Instantiate(rowPrefab, slotsContainer);
            rows.Add(row);
        }

        // Hide extra rows if needed (in case slots decrease someday)
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].gameObject.SetActive(i < count);
        }
    }

    private static void GetStatusAndTimer(double now, ExpeditionMission m, out string status, out string timer)
    {
        // Uses timing instead of MissionStatus, so it works with your current mission structure
        if (now < m.arriveTime)
        {
            status = "En route";
            timer = FormatSeconds(m.arriveTime - now);
            return;
        }

        if (now < m.returnTime)
        {
            status = "Returning";
            timer = FormatSeconds(m.returnTime - now);
            return;
        }

        status = "Complete";
        timer = "00:00";
    }

    private static string FormatSeconds(double seconds)
    {
        if (seconds < 0) seconds = 0;
        int s = (int)System.Math.Ceiling(seconds);
        int mm = s / 60;
        int ss = s % 60;
        return $"{mm:00}:{ss:00}";
    }
}