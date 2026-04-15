using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPageUI : MonoBehaviour
{
    [Header("References")]
    public GameStateHolder gameStateHolder;
    public string planetId = "home";

    [Header("Engineering")]
    public TMP_Text engineeringLevelText;
    public TMP_Text engineeringCostText;
    public Button engineeringButton;

    [Header("Weapon Systems")]
    public TMP_Text weaponLevelText;
    public TMP_Text weaponCostText;
    public Button weaponButton;

    [Header("Defense Systems")]
    public TMP_Text defenseLevelText;
    public TMP_Text defenseCostText;
    public Button defenseButton;

    [Header("Engine Tech")]
    public TMP_Text engineTechLevelText;
    public TMP_Text engineTechCostText;
    public Button engineTechButton;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;
        if (state == null) return;

        int engineeringLevel = state.research.GetLevel(ResearchType.Engineering);
        int weaponLevel = state.research.GetLevel(ResearchType.WeaponSystems);
        int defenseLevel = state.research.GetLevel(ResearchType.DefenseSystems);
        int engineTechLevel = state.research.GetLevel(ResearchType.EngineTech);

        if (engineeringLevelText != null)
            engineeringLevelText.text = $"Engineering Lv {engineeringLevel}";

        if (weaponLevelText != null)
            weaponLevelText.text = $"Weapon Systems Lv {weaponLevel}";

        if (defenseLevelText != null)
            defenseLevelText.text = $"Defense Systems Lv {defenseLevel}";

        if (engineTechLevelText != null)
            engineTechLevelText.text = $"Engine Tech Lv {engineTechLevel}";

        if (engineeringCostText != null)
            engineeringCostText.text =
                $"Metal: {Mathf.FloorToInt((float)ResearchSystem.GetMetalCost(state, ResearchType.Engineering))}  Crystal: {Mathf.FloorToInt((float)ResearchSystem.GetCrystalCost(state, ResearchType.Engineering))}";

        if (weaponCostText != null)
            weaponCostText.text =
                $"Metal: {Mathf.FloorToInt((float)ResearchSystem.GetMetalCost(state, ResearchType.WeaponSystems))}  Crystal: {Mathf.FloorToInt((float)ResearchSystem.GetCrystalCost(state, ResearchType.WeaponSystems))}";

        if (defenseCostText != null)
            defenseCostText.text =
                $"Metal: {Mathf.FloorToInt((float)ResearchSystem.GetMetalCost(state, ResearchType.DefenseSystems))}  Crystal: {Mathf.FloorToInt((float)ResearchSystem.GetCrystalCost(state, ResearchType.DefenseSystems))}";

        if (engineTechCostText != null)
            engineTechCostText.text =
                $"Metal: {Mathf.FloorToInt((float)ResearchSystem.GetMetalCost(state, ResearchType.EngineTech))}  Crystal: {Mathf.FloorToInt((float)ResearchSystem.GetCrystalCost(state, ResearchType.EngineTech))}";

        PlanetState planet = state.GetPlanet(planetId);
        if (planet != null)
        {
            if (engineeringButton != null)
                engineeringButton.interactable =
                    planet.metal >= ResearchSystem.GetMetalCost(state, ResearchType.Engineering) &&
                    planet.crystal >= ResearchSystem.GetCrystalCost(state, ResearchType.Engineering);

            if (weaponButton != null)
                weaponButton.interactable =
                    planet.metal >= ResearchSystem.GetMetalCost(state, ResearchType.WeaponSystems) &&
                    planet.crystal >= ResearchSystem.GetCrystalCost(state, ResearchType.WeaponSystems);

            if (defenseButton != null)
                defenseButton.interactable =
                    planet.metal >= ResearchSystem.GetMetalCost(state, ResearchType.DefenseSystems) &&
                    planet.crystal >= ResearchSystem.GetCrystalCost(state, ResearchType.DefenseSystems);

            if (engineTechButton != null)
                engineTechButton.interactable =
                    planet.metal >= ResearchSystem.GetMetalCost(state, ResearchType.EngineTech) &&
                    planet.crystal >= ResearchSystem.GetCrystalCost(state, ResearchType.EngineTech);
        }
    }

    public void OnClickUpgradeEngineering()
    {
        TryUpgrade(ResearchType.Engineering);
    }

    public void OnClickUpgradeWeaponSystems()
    {
        TryUpgrade(ResearchType.WeaponSystems);
    }

    public void OnClickUpgradeDefenseSystems()
    {
        TryUpgrade(ResearchType.DefenseSystems);
    }

    public void OnClickUpgradeEngineTech()
    {
        TryUpgrade(ResearchType.EngineTech);
    }

    private void TryUpgrade(ResearchType type)
    {
        GameState state = gameStateHolder != null ? gameStateHolder.state : null;
        if (state == null) return;

        if (ResearchSystem.TryResearch(state, type, planetId, out string error))
        {
            Refresh();
        }
        else
        {
            Debug.Log("Research failed: " + error);
        }
    }
}