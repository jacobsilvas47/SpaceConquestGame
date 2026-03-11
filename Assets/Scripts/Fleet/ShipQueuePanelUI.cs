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

    private class QueueDisplayGroup
    {
        public ShipType shipType;
        public int count;
        public double groupFinishTime;
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

    private void Rebuild()
    {
        ClearRows();

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            SetEmpty(true);
            return;
        }

        PlanetState p = gameStateHolder.state.GetPlanet(planetId);
        if (p == null || p.shipQueue == null || p.shipQueue.Count == 0)
        {
            SetEmpty(true);
            return;
        }

        List<QueueDisplayGroup> groups = BuildGroups(p.shipQueue);

        SetEmpty(groups.Count == 0);

        foreach (var g in groups)
        {
            ShipQueueRowUI row = Instantiate(rowPrefab, contentParent);
            spawnedRows.Add(row);

            double remaining = System.Math.Max(0, g.groupFinishTime - Time.time);

            row.Bind(
                GetShipDisplayName(g.shipType),
                g.count,
                remaining
            );
        }
    }

    private List<QueueDisplayGroup> BuildGroups(List<ShipQueueItem> queue)
    {
        List<QueueDisplayGroup> groups = new();

        if (queue == null || queue.Count == 0)
            return groups;

        QueueDisplayGroup current = null;

        for (int i = 0; i < queue.Count; i++)
        {
            ShipQueueItem item = queue[i];
            if (item == null) continue;

            if (current == null || current.shipType != item.shipType)
            {
                current = new QueueDisplayGroup
                {
                    shipType = item.shipType,
                    count = 1,
                    groupFinishTime = item.finishTime
                };
                groups.Add(current);
            }
            else
            {
                current.count++;
                current.groupFinishTime = item.finishTime;
            }
        }

        return groups;
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
        if (emptyStateObject) emptyStateObject.SetActive(isEmpty);
    }

    private string GetShipDisplayName(ShipType type)
    {
        switch (type)
        {
            case ShipType.Probe: return "Probe";
            case ShipType.SmallCargo: return "Small Cargo";
            case ShipType.LargeCargo: return "Large Cargo";
            case ShipType.BasicFighter: return "Basic Fighter";
            default: return type.ToString();
        }
    }
}