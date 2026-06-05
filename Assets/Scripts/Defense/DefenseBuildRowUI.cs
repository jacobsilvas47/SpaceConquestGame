using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseBuildRowUI : MonoBehaviour
{
    [Header("Defense")]
    public DefenseType defenseType;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI ownedText;
    public TextMeshProUGUI canBuildText;
    public TMP_InputField amountInput;
    public Button buildButton;
    public Button infoButton;

    [Header("Refs")]
    public GameStateHolder gameStateHolder;
    public string planetId = "home";
    public ResourceManager resourceManager;

    private void Start()
    {
        Refresh();

        if (buildButton != null)
            buildButton.onClick.AddListener(OnBuildClicked);

        if (infoButton != null)
            infoButton.onClick.AddListener(OnInfoClicked);
    }

    public void Refresh()
    {
        var defense = DefenseDatabase.Get(defenseType);
        if (defense == null) return;

        if (nameText) nameText.text = defense.displayName;

        if (costText)
        {
            costText.text =
                $"Cost: {NumberFormatter.Format(defense.metalCost)} M, " +
                $"{NumberFormatter.Format(defense.crystalCost)} C, " +
                $"{NumberFormatter.Format(defense.gasCost)} G";
        }

        int owned = 0;
        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            var planet = gameStateHolder.state.GetPlanet(planetId);
            if (planet != null)
                owned = planet.GetDefenseCount(defenseType);
        }

        if (ownedText)
            ownedText.text = $"Owned: {NumberFormatter.Format(owned)}";
            if (canBuildText && resourceManager != null)
        {
            int maxBuildable = resourceManager.GetMaxBuildableDefenses(defenseType);
            canBuildText.text = $"Can Build: {NumberFormatter.Format(maxBuildable)}";
        }

        bool requirementsMet = true;

        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            requirementsMet = RequirementUtility.MeetsRequirements(
                gameStateHolder.state,
                planetId,
                defense.requirements
            );
        }

        if (buildButton != null)
            buildButton.interactable = requirementsMet;
    }

    public int GetRequestedAmount()
    {
        if (amountInput == null) return 0;

        if (int.TryParse(amountInput.text, out int amount))
            return Mathf.Max(0, amount);

        return 0;
    }

    private void OnBuildClicked()
    {
        if (resourceManager == null) return;

        int amount = GetRequestedAmount();
        if (amount <= 0) return;

        bool success = resourceManager.TryBuildDefense(defenseType, amount);

        if (success && amountInput != null)
            amountInput.text = "";

        Refresh();
    }

    private void OnInfoClicked()
    {
        var defense = DefenseDatabase.Get(defenseType);
        if (defense == null) return;

        string title = defense.displayName;
        string body = BuildRequirementText(defense);

        if (DefenseInfoPopupUI.Instance != null)
            DefenseInfoPopupUI.Instance.Show(title, body);
        else
            Debug.Log(body);
    }

    private string BuildRequirementText(DefenseData defense)
    {
        if (defense.requirements == null || defense.requirements.Count == 0)
            return $"{defense.displayName} has no requirements.";

        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();

        foreach (Requirement req in defense.requirements)
        {
            parts.Add($"{GetRequirementDisplayName(req.type)} Lv {req.level}");
        }

        return $"{defense.displayName} requires: " + string.Join(", ", parts);
    }

    private string GetRequirementDisplayName(RequirementType type)
    {
        switch (type)
        {
            case RequirementType.OrbitalShipworks: return "Orbital Shipworks";
            case RequirementType.MetalRefinery: return "Metal Refinery";
            case RequirementType.CrystalMine: return "Crystal Mine";
            case RequirementType.GasExtractor: return "Gas Extractor";
            case RequirementType.Engineering: return "Engineering";
            case RequirementType.WeaponSystems: return "Weapon Systems";
            case RequirementType.DefenseSystems: return "Defense Systems";
            case RequirementType.EngineTech: return "Engine Tech";
            case RequirementType.LaserTech: return "Laser Tech";
            case RequirementType.ArmorTech: return "Armor Tech";
            default: return type.ToString();
        }
    }
}