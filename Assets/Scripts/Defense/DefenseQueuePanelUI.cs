using System.Collections.Generic;
using UnityEngine;

public class DefenseQueuePanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private DefenseQueueRowUI rowPrefab;
    [SerializeField] private GameObject emptyStateObject;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.2f;

    private float nextRefreshTime;
    private readonly List<DefenseQueueRowUI> spawnedRows = new();
    private int lastQueueCount = -1;

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

        PlanetState planet = GetPlanet();

        int queueCount = planet != null && planet.defenseBuildQueue != null
            ? planet.defenseBuildQueue.Count
            : 0;

        if (queueCount != lastQueueCount)
        {
            Rebuild();
            return;
        }

        RefreshExistingRows();
    }

    private PlanetState GetPlanet()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
            return null;

        return gameStateHolder.state.GetPlanet(planetId);
    }

    public void Rebuild()
    {
        ClearRows();

        PlanetState planet = GetPlanet();
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (planet == null || state == null || planet.defenseBuildQueue == null || planet.defenseBuildQueue.Count == 0)
        {
            lastQueueCount = 0;
            SetEmpty(true);
            return;
        }

        SetEmpty(false);

        double cumulativeRemaining = 0;

        for (int i = 0; i < planet.defenseBuildQueue.Count; i++)
        {
            DefenseBuildJob job = planet.defenseBuildQueue[i];

            if (i == 0 && job.started)
                cumulativeRemaining += System.Math.Max(0, job.completeTime - state.gameTime);
            else
                cumulativeRemaining += job.durationSeconds;

            DefenseQueueRowUI row = Instantiate(rowPrefab, contentParent);
            spawnedRows.Add(row);

            int index = i;

            row.Bind(
                job,
                state,
                index,
                cumulativeRemaining,
                CancelDefenseBuild
            );
        }

        lastQueueCount = planet.defenseBuildQueue.Count;
    }

    private void RefreshExistingRows()
    {
        PlanetState planet = GetPlanet();
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (planet == null || state == null || planet.defenseBuildQueue == null)
            return;

            double cumulativeRemaining = 0;

        for (int i = 0; i < spawnedRows.Count; i++)
        {
            if (spawnedRows[i] == null) continue;
            if (i >= planet.defenseBuildQueue.Count) continue;

            DefenseBuildJob job = planet.defenseBuildQueue[i];

            if (i == 0 && job.started)
                cumulativeRemaining += System.Math.Max(0, job.completeTime - state.gameTime);
            else
                cumulativeRemaining += job.durationSeconds;

            spawnedRows[i].UpdateDisplay(
                job,
                state,
                cumulativeRemaining
            );
        }
    }

    private void CancelDefenseBuild(int index)
    {
        PlanetState planet = GetPlanet();

        if (planet == null || planet.defenseBuildQueue == null)
            return;

        if (index < 0 || index >= planet.defenseBuildQueue.Count)
            return;

        planet.defenseBuildQueue.RemoveAt(index);

        Rebuild();
    }

    private void ClearRows()
    {
        for (int i = 0; i < spawnedRows.Count; i++)
        {
            if (spawnedRows[i] != null)
                Destroy(spawnedRows[i].gameObject);
        }

        spawnedRows.Clear();
    }

    private void SetEmpty(bool isEmpty)
    {
        if (emptyStateObject != null)
            emptyStateObject.SetActive(isEmpty);
    }
}