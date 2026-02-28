using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    [Header("Current Resources")]
    public double metal;
    public double crystal;
    public double gas;

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
    public TextMeshProUGUI probeText;

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

    void Start()
    {
        UpdateUI(); // so cost text shows immediately on play
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

    void UpdateUI()
    {
        double metalPerSec = GetMetalPerSecond();
        double crystalPerSec = GetCrystalPerSecond();
        double gasPerSec = GetGasPerSecond();
        double probeCountMult = GetProbeCountMultiplier();

        if (metalText) metalText.text = $"Metal: {System.Math.Floor(metal)}";
        if (crystalText) crystalText.text = $"Crystal: {System.Math.Floor(crystal)}";
        if (gasText) gasText.text = $"Gas: {System.Math.Floor(gas)}";
        if (probeText) probeText.text = $"Probes: {probes}  ({probeCountMult:0.##}x)";

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