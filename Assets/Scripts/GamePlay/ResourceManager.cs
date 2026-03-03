using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public GameStateHolder gameStateHolder;
    public string planetId = "home";

    private PlanetState GetPlanet()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return null;
        return gameStateHolder.state.GetPlanet(planetId);
    }

    private int GetTotalProbesFromGameState()
{
    if (gameStateHolder == null || gameStateHolder.state == null)
        return probes; // fallback safety

    var state = gameStateHolder.state;

    int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.Probe);
    int busy = GameStateQueries.GetBusyShips(state, ShipType.Probe);

    return stationed + busy;
}

    private double MetalFloor()   => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().metal);
    private double CrystalFloor() => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().crystal);
    private double GasFloor()     => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().gas);
        
    [Header("Current Resources (Debug Mirror of Planet, do not spend from these)")]
    [SerializeField] private double metal;
    [SerializeField] private double crystal;
    [SerializeField] private double gas;

    [Header("Ships")]
    public int smallCargo = 0;
    public int largeCargo = 0;

    [Header("Ships Busy (on missions)")]
    public int probesBusy = 0;
    public int smallCargoBusy = 0;
    public int largeCargoBusy = 0;

    [Header("Ship Costs (metal only, for now)")]
    public int smallCargoCostMetal = 200;
    public int largeCargoCostMetal = 600;

    [Header("Ship Cargo Capacity")]
    public int smallCargoCapacity = 500;
    public int largeCargoCapacity = 2000;

    [Header("Costs")] // Build Costs
    public int refineryCostMetal = 50; // Metal
    public int crystalMineCostMetal = 75; // Crystal
    public int gasExtractorCostMetal = 100; // Gas
    public int probeCostMetal = 100; // Probes
    public int probeCostCrystal = 100;
    public int probeCostGas = 100;

    [Header("Base Producers")]
    public int probes = 5;

    public int metalRefineries = 0;
    public int crystalMines = 0;
    public int gasExtractors = 0;

    [Header("Upgrades (multipliers)")]
    public float probeEfficiencyMult = 1f;
    public float planetCollectionMult = 1f;
    public float globalEconomyMult = 1f;

    [Header("UI Text (TextMeshProUGUI)")]
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI crystalText;
    public TextMeshProUGUI gasText;

    [Header("Ship UI Text")]
    public TextMeshProUGUI probesText;
    public TextMeshProUGUI smallCargoText;
    public TextMeshProUGUI largeCargoText;

    [Header("Cargo Build UI")]
    public Button buildSmallCargoButton;
    public Button buildLargeCargoButton;
    public TextMeshProUGUI smallCargoCostText;
    public TextMeshProUGUI largeCargoCostText;

    [Header("Cargo Row UI")]
    public TextMeshProUGUI smallCargoOwnedText;
    public TextMeshProUGUI largeCargoOwnedText;

    [Header("Build UI")]
    public Button buildRefineryButton;
    public TextMeshProUGUI refineryCostText;
    public TextMeshProUGUI actionStatusText;
    public Button buildCrystalMineButton;
    public Button buildGasExtractorButton;
    public TextMeshProUGUI crystalMineCostText;
    public TextMeshProUGUI gasExtractorCostText;

    [Header("Income UI Text (optional)")]
    public TextMeshProUGUI metalRateText;
    public TextMeshProUGUI crystalRateText;
    public TextMeshProUGUI gasRateText;

    [Header("Probe Multiplier Tuning")]
    public int startingProbes = 5;          // your default starting probes
    public double bonusPerExtraProbe = 0.02; // 2% per probe after startingProbes
    public double maxProbeMultiplier = 5.0;  // optional cap, keeps things sane

   [Header("Fleet UI")]
    public GameObject fleetPanel;
    public UnityEngine.UI.Button fleetButton;
    public UnityEngine.UI.Button closeFleetButton;

    public UnityEngine.UI.Button buildBasicFighterButton;   

    public TMPro.TextMeshProUGUI basicFighterOwnedText;
    public TMPro.TextMeshProUGUI basicFighterCostText;

    [Header("Fleet")]
    public int basicFighters = 0;
    public int basicFighterCostMetal = 50;

    [Header("Menu Panels")]
    public GameObject buildPanel;

    [Header("Menu Buttons")]
    public Button buildMenuButton;
    public Button closeBuildButton; 

void Start()
{
    // Start with clean screen
    if (buildPanel) buildPanel.SetActive(false);
    if (fleetPanel) fleetPanel.SetActive(false);

    // Open buttons
    if (buildMenuButton) buildMenuButton.onClick.AddListener(OpenBuild);
    if (fleetButton) fleetButton.onClick.AddListener(OpenFleet);

    // Close buttons
    if (closeBuildButton) closeBuildButton.onClick.AddListener(CloseBuild);
    if (closeFleetButton) closeFleetButton.onClick.AddListener(CloseFleet);

    if (gameStateHolder == null)
{
    gameStateHolder = FindFirstObjectByType<GameStateHolder>();
}

    UpdateUI();
    RefreshBasicFighterRow();
    RefreshCargoRows();
}

    void Update()
{
    double metalPerSec = GetMetalPerSecond();
    double crystalPerSec = GetCrystalPerSecond();
    double gasPerSec = GetGasPerSecond();

    var p = (gameStateHolder != null && gameStateHolder.state != null)
        ? gameStateHolder.state.GetPlanet(planetId)
        : null;

    if (p != null)
    {
        p.metal += metalPerSec * Time.deltaTime;
        p.crystal += crystalPerSec * Time.deltaTime;
        p.gas += gasPerSec * Time.deltaTime;

        // Mirror into inspector (debug)
        metal = p.metal;
        crystal = p.crystal;
        gas = p.gas;
    }

    UpdateUI();
}

    public int ProbesAvailable()     => Mathf.Max(0, probes - probesBusy);
    public int SmallCargoAvailable() => Mathf.Max(0, smallCargo - smallCargoBusy);
    public int LargeCargoAvailable() => Mathf.Max(0, largeCargo - largeCargoBusy);

    void UpdateUI()
    {
        double metalPerSec = GetMetalPerSecond();
        double crystalPerSec = GetCrystalPerSecond();
        double gasPerSec = GetGasPerSecond();
        double probeCountMult = GetProbeCountMultiplier();

        if (gameStateHolder != null && gameStateHolder.state != null)
    {
        var p = gameStateHolder.state.GetPlanet(planetId);
        if (p != null)
        {
            if (metalText) metalText.text = $"Metal: {System.Math.Floor(p.metal)}";
            if (crystalText) crystalText.text = $"Crystal: {System.Math.Floor(p.crystal)}";
            if (gasText) gasText.text = $"Gas: {System.Math.Floor(p.gas)}";
        }
    }
        
        if (probesText)
    {
        if (gameStateHolder != null && gameStateHolder.state != null)
        {
        var state = gameStateHolder.state;

        int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.Probe);
        int busy = GameStateQueries.GetBusyShips(state, ShipType.Probe);
        int total = stationed + busy;

        probesText.text = $"Probes: {stationed}/{total} (busy {busy}) ({probeCountMult:0.##}x)";
        }
    }
        if (smallCargoText) smallCargoText.text = $"Small Cargo: {SmallCargoAvailable()} (busy {smallCargoBusy})";
        if (largeCargoText) largeCargoText.text = $"Large Cargo: {LargeCargoAvailable()} (busy {largeCargoBusy})";

        if (smallCargoCostText) smallCargoCostText.text = $"Cost: {smallCargoCostMetal} Metal";
        if (largeCargoCostText) largeCargoCostText.text = $"Cost: {largeCargoCostMetal} Metal";

        if (buildSmallCargoButton)
            buildSmallCargoButton.interactable = MetalFloor() >= smallCargoCostMetal;

        if (buildLargeCargoButton)
            buildLargeCargoButton.interactable = MetalFloor() >= largeCargoCostMetal;

        if (metalRateText) metalRateText.text = $"Metal/s: {metalPerSec:0.##}";
        if (crystalRateText) crystalRateText.text = $"Crystal/s: {crystalPerSec:0.##}";
        if (gasRateText) gasRateText.text = $"Gas/s: {gasPerSec:0.##}";

        if (refineryCostText) refineryCostText.text = $"Cost: {refineryCostMetal} Metal";

        if (buildRefineryButton)
            buildRefineryButton.interactable   = MetalFloor() >= refineryCostMetal;

        if (buildCrystalMineButton)
            buildCrystalMineButton.interactable = MetalFloor() >= crystalMineCostMetal;

        if (buildGasExtractorButton)
            buildGasExtractorButton.interactable = MetalFloor() >= gasExtractorCostMetal;

        if (crystalMineCostText)
            crystalMineCostText.text = $"Cost: {crystalMineCostMetal} Metal";

        if (gasExtractorCostText)
            gasExtractorCostText.text = $"Cost: {gasExtractorCostMetal} Metal";

        RefreshBasicFighterRow();
        RefreshCargoRows();
    }

    // Building Methods
    public void TryBuildRefinery()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < refineryCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    p.metal -= refineryCostMetal;
    metalRefineries += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Refinery built";
    }

    UpdateUI();
}

public void TryBuildCrystalMine()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < crystalMineCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    p.metal -= crystalMineCostMetal;
    crystalMines += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Crystal Mine built";
    }

    UpdateUI();
}

public void TryBuildGasExtractor()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < gasExtractorCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    p.metal -= gasExtractorCostMetal;
    gasExtractors += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Gas Extractor built";
    }

    UpdateUI();
}

public void TryBuildProbe()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < probeCostMetal ||
        CrystalFloor() < probeCostCrystal ||
        GasFloor() < probeCostGas)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough resources";
        }
        return;
    }

    p.metal -= probeCostMetal;
    p.crystal -= probeCostCrystal;
    p.gas -= probeCostGas;

    // Add probe to GameState (this is the important line)
    p.AddStationed(ShipType.Probe, 1);

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Built 1 Probe";
    }

    UpdateUI();
}

// Fleet
public void OpenFleet()
{
    bool wasOpen = fleetPanel && fleetPanel.activeSelf;
    CloseAllMenus();
    if (fleetPanel) fleetPanel.SetActive(!wasOpen);
}

public void CloseFleet()     => CloseAllMenus();

public void TryBuildBasicFighter()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < basicFighterCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal for Basic Fighter";
        }
        return;
    }

    p.metal -= basicFighterCostMetal;
    basicFighters += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Basic Fighter Built";
    }

    UpdateUI();
    RefreshBasicFighterRow();
}

public void TryBuildSmallCargo()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < smallCargoCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal for Small Cargo";
        }
        return;
    }

    p.metal -= smallCargoCostMetal;
    smallCargo += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Small Cargo Built";
    }

    UpdateUI();
    RefreshCargoRows();
}

public void TryBuildLargeCargo()
{
    var p = GetPlanet();
    if (p == null) return;

    if (MetalFloor() < largeCargoCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal for Large Cargo";
        }
        return;
    }

    p.metal -= largeCargoCostMetal;
    largeCargo += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;
        actionStatusText.text = "Large Cargo Built";
    }

    UpdateUI();
    RefreshCargoRows();
}

void RefreshBasicFighterRow()
{
    if (basicFighterCostText)
        basicFighterCostText.text = $"Cost: {basicFighterCostMetal} Metal";

    if (basicFighterOwnedText)
        basicFighterOwnedText.text = $"Owned: {basicFighters}";

    if (buildBasicFighterButton)
        buildBasicFighterButton.interactable =
            MetalFloor() >= basicFighterCostMetal;
}

void RefreshCargoRows()
{
    if (smallCargoOwnedText) smallCargoOwnedText.text = $"Owned: {smallCargo}";
    if (largeCargoOwnedText) largeCargoOwnedText.text = $"Owned: {largeCargo}";
}

public void OpenBuild()
{
    bool wasOpen = buildPanel && buildPanel.activeSelf;
    CloseAllMenus();
    if (buildPanel) buildPanel.SetActive(!wasOpen);
}

void CloseAllMenus()
{
    if (buildPanel) buildPanel.SetActive(false);
    if (fleetPanel) fleetPanel.SetActive(false);
}

public void CloseBuild() => CloseAllMenus();

    double GetMetalPerSecond()
{
    double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
    double buildingMult = planetCollectionMult * globalEconomyMult;

    int totalProbes = GetTotalProbesFromGameState();
    double probeIncome = (totalProbes * 0.2) * probeMult;

    double buildingIncome = (metalRefineries * 2.0) * buildingMult;

    return probeIncome + buildingIncome;
}


double GetCrystalPerSecond()
{
    double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
    double buildingMult = planetCollectionMult * globalEconomyMult;

    int totalProbes = GetTotalProbesFromGameState();
    double probeIncome = (totalProbes * 0.1) * probeMult;

    double buildingIncome = (crystalMines * 1.5) * buildingMult;

    return probeIncome + buildingIncome;
}

double GetGasPerSecond()
{
    double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
    double buildingMult = planetCollectionMult * globalEconomyMult;

    int totalProbes = GetTotalProbesFromGameState();
    double probeIncome = (totalProbes * 0.05) * probeMult;

    double buildingIncome = (gasExtractors * 1.0) * buildingMult;

    return probeIncome + buildingIncome;
}

    double GetProbeCountMultiplier()
{
    int totalProbes = GetTotalProbesFromGameState();

    int extra = Mathf.Max(0, totalProbes - startingProbes);
    double mult = 1.0 + (extra * bonusPerExtraProbe);

    return System.Math.Min(mult, maxProbeMultiplier);
}

}