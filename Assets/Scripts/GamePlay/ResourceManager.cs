using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public GameStateHolder gameStateHolder;
    public string planetId = "home";
    
    [Header("Current Resources")]
    public double metal;
    public double crystal;
    public double gas;

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

        metal += metalPerSec * Time.deltaTime;
        crystal += crystalPerSec * Time.deltaTime;
        gas += gasPerSec * Time.deltaTime;

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

        if (metalText) metalText.text = $"Metal: {System.Math.Floor(metal)}";
        if (crystalText) crystalText.text = $"Crystal: {System.Math.Floor(crystal)}";
        if (gasText) gasText.text = $"Gas: {System.Math.Floor(gas)}";
        
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
            buildSmallCargoButton.interactable = System.Math.Floor(metal) >= smallCargoCostMetal;

        if (buildLargeCargoButton)
            buildLargeCargoButton.interactable = System.Math.Floor(metal) >= largeCargoCostMetal;

        if (metalRateText) metalRateText.text = $"Metal/s: {metalPerSec:0.##}";
        if (crystalRateText) crystalRateText.text = $"Crystal/s: {crystalPerSec:0.##}";
        if (gasRateText) gasRateText.text = $"Gas/s: {gasPerSec:0.##}";

        if (refineryCostText) refineryCostText.text = $"Cost: {refineryCostMetal} Metal";

        if (buildRefineryButton)
            buildRefineryButton.interactable = System.Math.Floor(metal) >= refineryCostMetal;

        if (buildCrystalMineButton)
            buildCrystalMineButton.interactable = System.Math.Floor(metal) >= crystalMineCostMetal;

        if (buildGasExtractorButton)
            buildGasExtractorButton.interactable = System.Math.Floor(metal) >= gasExtractorCostMetal;

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
    // use Floor so you can't spend "partial" metal like 49.7
    if (System.Math.Floor(metal) < refineryCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;          
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    metal -= refineryCostMetal;
    metalRefineries += 1;

    if (actionStatusText)
    {
        actionStatusText.color = Color.green;           
        actionStatusText.text = "Refinery built";
    }

    UpdateUI(); // instant feedback
}

public void TryBuildCrystalMine()
{
    if (System.Math.Floor(metal) < crystalMineCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    metal -= crystalMineCostMetal;
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
    if (System.Math.Floor(metal) < gasExtractorCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal";
        }
        return;
    }

    metal -= gasExtractorCostMetal;
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
    // Prevent spending partial resources if you're using doubles
    if (System.Math.Floor(metal) < probeCostMetal ||
        System.Math.Floor(crystal) < probeCostCrystal ||
        System.Math.Floor(gas) < probeCostGas)
    {
        if (actionStatusText) actionStatusText.text = "Not enough resources";
        return;
    }

    metal -= probeCostMetal;
    crystal -= probeCostCrystal;
    gas -= probeCostGas;

    probes += 1;

    if (actionStatusText) actionStatusText.text = "Built 1 Probe";
    UpdateUI(); // if you already have this function, keep using it
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
    if (System.Math.Floor(metal) < basicFighterCostMetal)
        return;

    metal -= basicFighterCostMetal;
    basicFighters += 1;

    UpdateUI();
    RefreshBasicFighterRow();
}

public void TryBuildSmallCargo()
{
    if (System.Math.Floor(metal) < smallCargoCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal for Small Cargo";
        }
        return;
    }

    metal -= smallCargoCostMetal;
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
    if (System.Math.Floor(metal) < largeCargoCostMetal)
    {
        if (actionStatusText)
        {
            actionStatusText.color = Color.red;
            actionStatusText.text = "Not enough Metal for Large Cargo";
        }
        return;
    }

    metal -= largeCargoCostMetal;
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
            System.Math.Floor(metal) >= basicFighterCostMetal;
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

    double probeIncome = (probes * 0.2) * probeMult;
    double buildingIncome = (metalRefineries * 2.0) * buildingMult;

    return probeIncome + buildingIncome;
}


double GetCrystalPerSecond()
{
    double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
    double buildingMult = planetCollectionMult * globalEconomyMult;

    double probeIncome = (probes * 0.1) * probeMult;
    double buildingIncome = (crystalMines * 1.5) * buildingMult;

    return probeIncome + buildingIncome;
}

double GetGasPerSecond()
{
    double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
    double buildingMult = planetCollectionMult * globalEconomyMult;

    double probeIncome = (probes * 0.05) * probeMult;
    double buildingIncome = (gasExtractors * 1.0) * buildingMult;

    return probeIncome + buildingIncome;
}

    double GetProbeCountMultiplier()
{
    int extra = Mathf.Max(0, probes - startingProbes); // probes beyond baseline
    double mult = 1.0 + (extra * bonusPerExtraProbe);  // linear gentle growth
    return System.Math.Min(mult, maxProbeMultiplier);   // cap it
}

}