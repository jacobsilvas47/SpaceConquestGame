using System.Collections.Generic;
using UnityEngine;

public class BuildingQueuePanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private BuildingQueueRowUI rowPrefab;
    [SerializeField] private GameObject emptyStateObject;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.2f;

    private float nextRefreshTime;
    private readonly List<BuildingQueueRowUI> spawnedRows = new();

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

        int queueCount = planet != null && planet.buildingUpgradeQueue != null
            ? planet.buildingUpgradeQueue.Count
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

        if (planet == null || state == null || planet.buildingUpgradeQueue == null || planet.buildingUpgradeQueue.Count == 0)
        {
            lastQueueCount = 0;
            SetEmpty(true);
            return;
        }

        SetEmpty(false);

        for (int i = 0; i < planet.buildingUpgradeQueue.Count; i++)
        {
            BuildingQueueRowUI row = Instantiate(rowPrefab, contentParent);
            spawnedRows.Add(row);

            int index = i;

            row.Bind(
                planet.buildingUpgradeQueue[i],
                state,
                index,
                CancelBuildingUpgrade
            );
        }

        lastQueueCount = planet.buildingUpgradeQueue.Count;
    }

    private void RefreshExistingRows()
    {
        PlanetState planet = GetPlanet();
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (planet == null || state == null || planet.buildingUpgradeQueue == null)
            return;

        for (int i = 0; i < spawnedRows.Count; i++)
        {
            if (spawnedRows[i] == null) continue;
            if (i >= planet.buildingUpgradeQueue.Count) continue;

            spawnedRows[i].UpdateDisplay(planet.buildingUpgradeQueue[i], state);
        }
    }

    private void CancelBuildingUpgrade(int index)
    {
        PlanetState planet = GetPlanet();

        if (planet == null || planet.buildingUpgradeQueue == null)
            return;

        if (index < 0 || index >= planet.buildingUpgradeQueue.Count)
            return;

        planet.buildingUpgradeQueue.RemoveAt(index);

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