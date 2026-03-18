using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AITargetsPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private AITargetRowUI rowPrefab;

    [Header("Attack Setup")]
    [SerializeField] private string originPlanetId = "home";
    [SerializeField] private double attackDurationSeconds = 30;

    [Header("Test Fleet")]
    [SerializeField] private int probesToSend = 0;
    [SerializeField] private int basicFightersToSend = 5;
    [SerializeField] private int smallCargoToSend = 2;
    [SerializeField] private int largeCargoToSend = 0;

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private readonly List<AITargetRowUI> rows = new();

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
            SetStatus("UI references missing.", Color.red);
            return;
        }

        var state = gameStateHolder.state;

        if (state.aiTargets == null || state.aiTargets.Count == 0)
        {
            SetStatus("No AI targets available.", Color.yellow);
            return;
        }

        foreach (var kvp in state.aiTargets)
        {
            AttackTarget target = kvp.Value;
            if (target == null) continue;

            AITargetRowUI row = Instantiate(rowPrefab, contentRoot);
            row.Bind(target, OnAttackTargetClicked);
            rows.Add(row);
        }

        SetStatus($"Loaded {rows.Count} AI targets.", Color.white);
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

    private void OnAttackTargetClicked(AttackTarget target)
    {
        if (target == null)
        {
            SetStatus("Target was null.", Color.red);
            return;
        }

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            SetStatus("GameState not found.", Color.red);
            return;
        }

        GameState state = gameStateHolder.state;

        FleetComposition comp = new FleetComposition
        {
            probes = probesToSend,
            basicFighters = basicFightersToSend,
            smallCargo = smallCargoToSend,
            largeCargo = largeCargoToSend
        };

        if (comp.TotalShips() <= 0)
        {
            SetStatus("Test fleet is empty.", Color.red);
            return;
        }

        string fleetId = FleetFactory.TryCreateFleetFromPlanet(state, originPlanetId, comp);
        if (string.IsNullOrEmpty(fleetId))
        {
            SetStatus("Could not create fleet. Check stationed ships on the origin planet.", Color.red);
            return;
        }

        string missionId = MissionEngine.TrySendAttack(
            state,
            fleetId,
            target.targetId,
            TargetType.AITarget,
            attackDurationSeconds
        );

        if (string.IsNullOrEmpty(missionId))
        {
            SetStatus($"Failed to launch attack on {target.displayName}.", Color.red);
            return;
        }

        SetStatus($"Attack launched on {target.displayName}.", Color.green);
        Debug.Log($"[AI TARGETS PANEL] Sent attack mission {missionId} to {target.displayName}");

        Rebuild();
    }

    private void SetStatus(string text, Color color)
    {
        if (statusText == null) return;

        statusText.text = text;
        statusText.color = color;
    }
}