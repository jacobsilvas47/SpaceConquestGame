using System.Collections.Generic;
using UnityEngine;

public class ResearchQueuePanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ResearchQueueRowUI rowPrefab;
    [SerializeField] private GameObject emptyStateObject;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.2f;

    private float nextRefreshTime;
    private readonly List<ResearchQueueRowUI> spawnedRows = new();
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

        int queueCount = GetQueueCount();

        if (queueCount != lastQueueCount)
        {
            Rebuild();
            return;
        }

        RefreshExistingRows();
    }

    private int GetQueueCount()
    {
        if (gameStateHolder == null || gameStateHolder.state == null || gameStateHolder.state.research == null)
            return 0;

        return gameStateHolder.state.research.researchQueue != null
            ? gameStateHolder.state.research.researchQueue.Count
            : 0;
    }

    public void Rebuild()
    {
        ClearRows();

        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (state == null || state.research == null || state.research.researchQueue == null || state.research.researchQueue.Count == 0)
        {
            lastQueueCount = 0;
            SetEmpty(true);
            return;
        }

        SetEmpty(false);

        for (int i = 0; i < state.research.researchQueue.Count; i++)
        {
            ResearchQueueRowUI row = Instantiate(rowPrefab, contentParent);
            spawnedRows.Add(row);

            int index = i;

            row.Bind(
                state.research.researchQueue[i],
                state,
                index,
                CancelResearch
            );
        }

        lastQueueCount = state.research.researchQueue.Count;
    }

    private void RefreshExistingRows()
    {
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (state == null || state.research == null || state.research.researchQueue == null)
            return;

        for (int i = 0; i < spawnedRows.Count; i++)
        {
            if (spawnedRows[i] == null) continue;
            if (i >= state.research.researchQueue.Count) continue;

            spawnedRows[i].UpdateDisplay(state.research.researchQueue[i], state);
        }
    }

    private void CancelResearch(int index)
    {
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;

        if (state == null || state.research == null || state.research.researchQueue == null)
            return;

        if (index < 0 || index >= state.research.researchQueue.Count)
            return;

        state.research.researchQueue.RemoveAt(index);

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