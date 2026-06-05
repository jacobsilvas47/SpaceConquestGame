using System.Collections.Generic;
using UnityEngine;

public class FleetBuildPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string planetId = "home";

    [Header("Prefab Setup")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ShipBuildCardUI shipBuildCardPrefab;

    [Header("Ship Icons")]
    [SerializeField] private Sprite probeIcon;

    private ShipCategory currentCategory = ShipCategory.All;
    private readonly List<ShipBuildCardUI> spawnedCards = new();

    private void Start()
    {
        Rebuild();
    }

    private void OnEnable()
    {
        Rebuild();
    }

    private void Update()
    {
        foreach (ShipBuildCardUI card in spawnedCards)
        {
            if (card != null)
                card.Refresh();
        }
    }

    public void Rebuild()
    {
        ClearCards();

        CreateCard(ShipType.Probe, ShipCategory.ScoutsAndUtility, probeIcon);

        CreateCard(ShipType.SmallCargo, ShipCategory.CargoAndLogistics, null);
        CreateCard(ShipType.LargeCargo, ShipCategory.CargoAndLogistics, null);

        CreateCard(ShipType.BasicFighter, ShipCategory.LightCombat, null);
        CreateCard(ShipType.Interceptor, ShipCategory.LightCombat, null);

        CreateCard(ShipType.ColonyShip, ShipCategory.Colonization, null);

        ApplyCategoryFilter(currentCategory);
    }

    private void CreateCard(ShipType shipType, ShipCategory category, Sprite icon)
    {
        if (contentParent == null || shipBuildCardPrefab == null)
            return;

        ShipBuildCardUI card = Instantiate(shipBuildCardPrefab, contentParent);

        card.Setup(
            shipType,
            category,
            icon,
            resourceManager,
            gameStateHolder,
            planetId
        );

        spawnedCards.Add(card);
    }

    private void ClearCards()
    {
        foreach (ShipBuildCardUI card in spawnedCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        spawnedCards.Clear();
    }

    public void OnClickBuildAll()
    {
        foreach (ShipBuildCardUI card in spawnedCards)
        {
            if (card != null)
                card.TryBuildFromInput();
        }
    }

    public void ShowAll()
    {
        ApplyCategoryFilter(ShipCategory.All);
    }

    public void ShowScoutsAndUtility()
    {
        ApplyCategoryFilter(ShipCategory.ScoutsAndUtility);
    }

    public void ShowCargoAndLogistics()
    {
        ApplyCategoryFilter(ShipCategory.CargoAndLogistics);
    }

    public void ShowColonization()
    {
        ApplyCategoryFilter(ShipCategory.Colonization);
    }

    public void ShowLightCombat()
    {
        ApplyCategoryFilter(ShipCategory.LightCombat);
    }

    public void ShowFrigates()
    {
        ApplyCategoryFilter(ShipCategory.Frigates);
    }

    public void ShowSpecialistCombat()
    {
        ApplyCategoryFilter(ShipCategory.SpecialistCombat);
    }

    public void ShowCapitalShips()
    {
        ApplyCategoryFilter(ShipCategory.CapitalShips);
    }

    private void ApplyCategoryFilter(ShipCategory category)
    {
        currentCategory = category;

        foreach (ShipBuildCardUI card in spawnedCards)
        {
            if (card == null) continue;

            bool shouldShow =
                category == ShipCategory.All ||
                card.GetCategory() == category;

            card.gameObject.SetActive(shouldShow);
        }
    }
}