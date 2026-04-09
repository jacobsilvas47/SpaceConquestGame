using System.Text;
using TMPro;
using UnityEngine;

public class ShipRequirementsPopupUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text requirementsText;
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string currentPlanetId = "home";

    public void ShowForShip(ShipType shipType)
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            Debug.LogError("ShipRequirementsPopupUI: Missing GameStateHolder or GameState.");
            return;
        }

        GameState state = gameStateHolder.state;
        PlanetState planet = state.GetPlanet(currentPlanetId);

        if (planet == null)
        {
            Debug.LogError($"ShipRequirementsPopupUI: No planet found for id '{currentPlanetId}'");
            return;
        }

        ShipData data = ShipDatabase.Get(shipType);
        if (data == null)
        {
            Debug.LogError($"ShipRequirementsPopupUI: No ShipData found for {shipType}");
            return;
        }

        if (titleText != null)
            titleText.text = $"{data.displayName} Requirements";

        if (requirementsText != null)
            requirementsText.text = BuildRequirementsText(state, data);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private string BuildRequirementsText(GameState state, ShipData data)
    {
        StringBuilder sb = new StringBuilder();

        if (data.requirements == null || data.requirements.Count == 0)
        {
            sb.AppendLine("No unlock requirements.");
            return sb.ToString();
        }

        foreach (Requirement req in data.requirements)
        {
            int current = RequirementQueries.GetLevel(state, currentPlanetId, req.type);
            bool met = current >= req.level;

            string status = met ? "✓" : "✗";
            string reqName = GetRequirementDisplayName(req.type);

            sb.AppendLine($"{status} {reqName} Lv {req.level}  (Current: {current})");
        }

        return sb.ToString();
    }

    private string GetRequirementDisplayName(RequirementType type)
    {
        switch (type)
        {
            case RequirementType.MetalRefinery: return "Metal Refinery";
            case RequirementType.CrystalMine: return "Crystal Mine";
            case RequirementType.GasExtractor: return "Gas Extractor";
            case RequirementType.OrbitalShipworks: return "Orbital Shipworks";
            case RequirementType.Barracks: return "Barracks";
            case RequirementType.ResearchLab: return "Research Lab";
            case RequirementType.LaserTech: return "Laser Tech";
            case RequirementType.ArmorTech: return "Armor Tech";
            case RequirementType.EngineTech: return "Engine Tech";
            default: return type.ToString();
        }
    }
}