using System.Collections.Generic;
using UnityEngine;

public class ShipQueuePanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ShipQueueRowUI rowPrefab;
    [SerializeField] private GameObject emptyStateObject;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.2f;

    private float nextRefreshTime;
    private readonly List<ShipQueueRowUI> spawnedRows = new();
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

        int queueCount = planet != null && planet.shipQueue != null
            ? planet.shipQueue.Count
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

        if (planet == null || planet.shipQueue == null || planet.shipQueue.Count == 0)
        {
            lastQueueCount = 0;
            SetEmpty(true);
            return;
        }

        SetEmpty(false);

        for (int i = 0; i < planet.shipQueue.Count; i++)
        {
            ShipQueueRowUI row = Instantiate(rowPrefab, contentParent);
            spawnedRows.Add(row);

            int index = i;

            row.Bind(
                planet.shipQueue[i],
                index,
                CancelShipBuild
            );
        }

        lastQueueCount = planet.shipQueue.Count;
    }

    private void RefreshExistingRows()
    {
        PlanetState planet = GetPlanet();

        if (planet == null || planet.shipQueue == null)
            return;

        for (int i = 0; i < spawnedRows.Count; i++)
        {
            if (spawnedRows[i] == null) continue;
            if (i >= planet.shipQueue.Count) continue;

            spawnedRows[i].UpdateDisplay(planet.shipQueue[i]);
        }
    }

    private void CancelShipBuild(int index)
    {
        PlanetState planet = GetPlanet();

        if (planet == null || planet.shipQueue == null)
            return;

        if (index < 0 || index >= planet.shipQueue.Count)
            return;

        planet.shipQueue.RemoveAt(index);

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