using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenseBuildPanelUI : MonoBehaviour
{
    [Header("Rows")]
    [SerializeField] private List<DefenseBuildRowUI> rows = new();

    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("Summary UI")]
    [SerializeField] private TextMeshProUGUI totalAttackText;
    [SerializeField] private TextMeshProUGUI totalHPText;
    [SerializeField] private TextMeshProUGUI totalShieldText;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.25f;

    private float nextRefreshTime;

    private void OnEnable()
    {
        RefreshAll();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime) return;
        nextRefreshTime = Time.unscaledTime + refreshInterval;

        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var row in rows)
        {
            if (row != null)
                row.Refresh();
        }

        if (gameStateHolder == null || gameStateHolder.state == null)
            return;

        var planet = gameStateHolder.state.GetPlanet(planetId);
        if (planet == null)
            return;

        if (totalAttackText)
            totalAttackText.text = $"Planet Attack: {NumberFormatter.Format(planet.GetTotalDefenseAttack())}";

        if (totalHPText)
            totalHPText.text = $"Planet HP: {NumberFormatter.Format(planet.GetTotalDefenseHP())}";

        if (totalShieldText)
            totalShieldText.text = $"Planet Shield: {NumberFormatter.Format(planet.GetTotalDefenseShield())}";
    }
}