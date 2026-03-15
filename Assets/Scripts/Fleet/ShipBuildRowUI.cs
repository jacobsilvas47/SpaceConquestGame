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
        var ship = ShipDatabase.Get(shipType);
        if (ship == null) return;

        if (nameText) nameText.text = ship.displayName;

        if (costText)
        {
            costText.text =
                $"Cost: {NumberFormatter.Format(ship.metalCost)} M, " +
                $"{NumberFormatter.Format(ship.crystalCost)} C, " +
                $"{NumberFormatter.Format(ship.gasCost)} G";
        }

        int owned = 0;
        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            owned = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);
        }

        if (ownedText)
            ownedText.text = $"Owned: {NumberFormatter.Format(owned)}";
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
}