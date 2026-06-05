using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipBuildCardUI : MonoBehaviour
{
    [Header("Ship")]
    [SerializeField] private ShipType shipType;
    [SerializeField] private ShipCategory category;

    [Header("UI References")]
    [SerializeField] private Image shipIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TMP_InputField quantityInput;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button infoButton;

    private ResourceManager resourceManager;
    private GameStateHolder gameStateHolder;
    private string planetId = "home";

    public void Setup(
        ShipType type,
        ShipCategory shipCategory,
        Sprite icon,
        ResourceManager manager,
        GameStateHolder holder,
        string planet)
    {
        shipType = type;
        category = shipCategory;
        resourceManager = manager;
        gameStateHolder = holder;
        planetId = planet;

        if (shipIcon != null)
            shipIcon.sprite = icon;

        if (quantityInput != null && string.IsNullOrWhiteSpace(quantityInput.text))
            quantityInput.text = "1";

        if (buildButton != null)
        {
            buildButton.onClick.RemoveAllListeners();
            buildButton.onClick.AddListener(TryBuildFromInput);
        }

        Refresh();
    }

    public ShipCategory GetCategory()
    {
        return category;
    }

    public void Refresh()
    {
        ShipData data = ShipDatabase.Get(shipType);
        if (data == null) return;

        if (nameText != null)
            nameText.text = data.displayName;

        if (costText != null)
        {
            costText.text =
                $"{NumberFormatter.Format(data.metalCost)} M / " +
                $"{NumberFormatter.Format(data.crystalCost)} C / " +
                $"{NumberFormatter.Format(data.gasCost)} G";
        }

        if (ownedText != null)
        {
            int owned = 0;

            if (gameStateHolder != null && gameStateHolder.state != null)
                owned = GameStateQueries.GetStationedShips(gameStateHolder.state, planetId, shipType);

            ownedText.text = $"Owned: {NumberFormatter.Format(owned)}";
        }
    }

    public void TryBuildFromInput()
    {
        if (resourceManager == null) return;

        int amount = GetRequestedAmount();
        if (amount <= 0) return;

        bool success = resourceManager.TryQueueShipBuild(shipType, amount);

        if (success)
        {
            ClearInput();
            Refresh();
        }
    }

    public int GetRequestedAmount()
    {
        if (quantityInput == null) return 1;

        if (int.TryParse(quantityInput.text, out int amount))
            return Mathf.Max(0, amount);

        return 0;
    }

    public void ClearInput()
    {
        if (quantityInput != null)
            quantityInput.text = "1";
    }
}