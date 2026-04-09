using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipBuildRowUI : MonoBehaviour
{
    [Header("Ship")]
    public ShipType shipType;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI ownedText;
    public TextMeshProUGUI canBuildText;
    public TextMeshProUGUI requirementText;
    public TMP_InputField amountInput;
    public Button buildButton;
    public Button infoButton;

    [Header("Refs")]
    public GameStateHolder gameStateHolder;
    public string planetId = "home";
    public ResourceManager resourceManager;
    public ShipRequirementsPopupUI requirementsPopup;

    private float nextRefresh;

    private void Start()
    {
        Refresh();

        if (buildButton != null)
        {
            buildButton.onClick.RemoveAllListeners();
            buildButton.onClick.AddListener(OnBuildClicked);
        }

        if (infoButton != null)
        {
            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(OnInfoClicked);
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + 0.25f;
        Refresh();
    }

    public void Refresh()
    {
        var ship = ShipDatabase.Get(shipType);
        if (ship == null) return;

        if (nameText)
            nameText.text = ship.displayName;

        if (costText)
        {
            costText.text =
                $"Cost: {NumberFormatter.Format(ship.metalCost)} M, " +
                $"{NumberFormatter.Format(ship.crystalCost)} C, " +
                $"{NumberFormatter.Format(ship.gasCost)} G";
        }

        int owned = 0;
        bool requirementsMet = false;

        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            owned = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);

            requirementsMet = RequirementUtility.MeetsRequirements(
                gameStateHolder.state,
                planetId,
                ship.requirements
            );
        }

        if (ownedText)
            ownedText.text = $"Owned: {NumberFormatter.Format(owned)}";

        if (canBuildText && resourceManager != null)
        {
            int maxBuildable = resourceManager.GetMaxBuildableShips(shipType);
            canBuildText.text = $"Can Build: {NumberFormatter.Format(maxBuildable)}";
        }

        if (buildButton != null)
            buildButton.interactable = requirementsMet;

        if (requirementText != null)
        {
            requirementText.text = "";
            requirementText.gameObject.SetActive(false);
        }
    }

    private void OnBuildClicked()
    {
        if (resourceManager == null)
        {
            Debug.LogError($"[ShipBuildRowUI] ResourceManager missing on {name}");
            return;
        }

        if (amountInput == null)
        {
            Debug.LogError($"[ShipBuildRowUI] Amount input missing on {name}");
            return;
        }

        if (!int.TryParse(amountInput.text, out int amount) || amount <= 0)
        {
            Debug.LogWarning($"[ShipBuildRowUI] Invalid amount '{amountInput.text}' for {shipType}");
            return;
        }

        bool success = resourceManager.TryQueueShipBuild(shipType, amount);

        if (success)
        {
            amountInput.text = "";
            Refresh();
        }
    }

    private void OnInfoClicked()
    {
        if (requirementsPopup == null)
        {
            Debug.LogWarning($"[ShipBuildRowUI] Requirements popup is not assigned for {shipType}");
            return;
        }

        requirementsPopup.ShowForShip(shipType);
    }

    public int GetRequestedAmount()
    {
        if (amountInput == null) return 0;
        if (!int.TryParse(amountInput.text, out int amount)) return 0;
        return Mathf.Max(0, amount);
    }

    public ShipCategory GetCategory()
    {
        return ShipCategoryDatabase.GetCategory(shipType);
    }

    public void ClearInput()
    {
        if (amountInput != null)
            amountInput.text = "";
    }

    private string BuildRequirementText(ShipData ship)
    {
        if (ship == null || ship.requirements == null || ship.requirements.Count == 0)
            return "";

        List<string> parts = new List<string>();

        foreach (var req in ship.requirements)
        {
            parts.Add($"{GetRequirementDisplayName(req.type)} Lv {req.level}");
        }

        return "Requires: " + string.Join(", ", parts);
    }

    private string GetRequirementDisplayName(RequirementType type)
    {
        switch (type)
        {
            case RequirementType.OrbitalShipworks: return "Orbital Shipworks";
            case RequirementType.MetalRefinery: return "Metal Refinery";
            case RequirementType.CrystalMine: return "Crystal Mine";
            case RequirementType.GasExtractor: return "Gas Extractor";
            case RequirementType.Barracks: return "Barracks";
            case RequirementType.ResearchLab: return "Research Lab";
            case RequirementType.LaserTech: return "Laser Tech";
            case RequirementType.ArmorTech: return "Armor Tech";
            case RequirementType.EngineTech: return "Engine Tech";
            default: return type.ToString();
        }
    }
}