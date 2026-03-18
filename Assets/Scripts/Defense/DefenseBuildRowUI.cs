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

    [Header("Refs")]
    public GameStateHolder gameStateHolder;
    public string planetId = "home";
    public ResourceManager resourceManager;

    private void Start()
    {
        Refresh();

        if (buildButton != null)
            buildButton.onClick.AddListener(OnBuildClicked);
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
}