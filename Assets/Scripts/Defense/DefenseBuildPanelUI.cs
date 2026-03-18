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

        int totalAttack = planet.GetTotalDefenseAttack() + GetStationedFleetAttack();
        int totalHP = planet.GetTotalDefenseHP() + GetStationedFleetHP();
        int totalShield = planet.GetTotalDefenseShield() + GetStationedFleetShield();

        if (totalAttackText)
            totalAttackText.text = $"Planet Attack: {NumberFormatter.Format(totalAttack)}";

        if (totalHPText)
            totalHPText.text = $"Planet HP: {NumberFormatter.Format(totalHP)}";

        if (totalShieldText)
            totalShieldText.text = $"Planet Shield: {NumberFormatter.Format(totalShield)}";
    }

    private int GetStationedFleetAttack()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
            return 0;

        int total = 0;

        foreach (ShipType shipType in System.Enum.GetValues(typeof(ShipType)))
        {
            if (shipType == ShipType.None)
                continue;

            int count = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);
            if (count <= 0)
                continue;

            ShipData ship = ShipDatabase.Get(shipType);
            if (ship == null)
                continue;

            total += count * ship.attack;
        }

        return total;
    }

    private int GetStationedFleetHP()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
            return 0;

        int total = 0;

        foreach (ShipType shipType in System.Enum.GetValues(typeof(ShipType)))
        {
            if (shipType == ShipType.None)
                continue;

            int count = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);
            if (count <= 0)
                continue;

            ShipData ship = ShipDatabase.Get(shipType);
            if (ship == null)
                continue;

            total += count * ship.maxHp;
        }

        return total;
    }

    private int GetStationedFleetShield()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
            return 0;

        int total = 0;

        foreach (ShipType shipType in System.Enum.GetValues(typeof(ShipType)))
        {
            if (shipType == ShipType.None)
                continue;

            int count = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);
            if (count <= 0)
                continue;

            ShipData ship = ShipDatabase.Get(shipType);
            if (ship == null)
                continue;

            total += count * ship.defense;
        }

        return total;
    }
}