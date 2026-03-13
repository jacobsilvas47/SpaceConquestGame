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
        if (costText) costText.text = $"Cost: {ship.metalCost} M, {ship.crystalCost} C, {ship.gasCost} G";

        int owned = 0;
        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            owned = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);
        }

        if (ownedText) ownedText.text = $"Owned: {owned}";
    }

    private void OnBuildClicked()
    {
        if (resourceManager == null) return;
        if (amountInput == null) return;

        if (!int.TryParse(amountInput.text, out int amount) || amount <= 0)
            return;

        resourceManager.TryQueueShipBuild(shipType, amount);
        amountInput.text = "";
        Refresh();
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

