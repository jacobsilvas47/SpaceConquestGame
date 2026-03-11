using System;
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

    // Debug Headers
    [Header("DEBUG CHEATS (Editor Only)")]
    public double debugAddAmount = 50000;

    // Headers
    [Header("Current Resources (Debug Mirror of Planet, do not spend from these)")]
    [SerializeField] private double metal;
    [SerializeField] private double crystal;
    [SerializeField] private double gas;

    /*
    [Header("Ships")]
    public int smallCargo = 0;
    public int largeCargo = 0;

    [Header("Ships Busy (on missions)")]
    public int probesBusy = 0;
    public int smallCargoBusy = 0;
    public int largeCargoBusy = 0;
    */

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

    [Header("Level")] // Build Costs
    [SerializeField] private TextMeshProUGUI refineryLevelText;
    [SerializeField] private TextMeshProUGUI crystalLevelText;
    [SerializeField] private TextMeshProUGUI gasLevelText;
    [SerializeField] private TextMeshProUGUI probeLevelText;

    [Header("Base Producers")]
    public int probes = 5;

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
    public Button buildCrystalMineButton;
    public Button buildGasExtractorButton;
    public Button buildProbeButton;

    public TextMeshProUGUI refineryCostText;
    public TextMeshProUGUI crystalMineCostText;
    public TextMeshProUGUI gasExtractorCostText;
    public TextMeshProUGUI probeCostText;

    [Header("Status UI")]
    public TextMeshProUGUI actionStatusText;   // success / error messages
    public TextMeshProUGUI upgradeStatusText;  // building timer
    public TextMeshProUGUI shipQueueText;      // fleet queue timer

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
    public int basicFighterCostMetal = 50;

    [Header("Menu Panels")]
    public GameObject buildPanel;

    [Header("Menu Buttons")]
    public Button buildMenuButton;
    public Button closeBuildButton; 

    void Start()
    {
        // Start with clean screen
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
            // Tick timed systems first
            BuildingUpgradeSystem.TickPlanet(p);
            TickShipQueue(p);

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

    public int ProbesAvailable()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return 0;
        var state = gameStateHolder.state;

        int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.Probe);
        int busy = GameStateQueries.GetBusyShips(state, ShipType.Probe);
        return Mathf.Max(0, stationed); // "available" = stationed (busy are not stationed)
    }

    public int SmallCargoAvailable()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return 0;
        var state = gameStateHolder.state;

        int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.SmallCargo);
        int busy = GameStateQueries.GetBusyShips(state, ShipType.SmallCargo);
        return Mathf.Max(0, stationed);
    }

    public int LargeCargoAvailable()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return 0;
        var state = gameStateHolder.state;

        int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.LargeCargo);
        int busy = GameStateQueries.GetBusyShips(state, ShipType.LargeCargo);
        return Mathf.Max(0, stationed);
    }

        void UpdateUI()
    {
        double metalPerSec = GetMetalPerSecond();
        double crystalPerSec = GetCrystalPerSecond();
        double gasPerSec = GetGasPerSecond();
        double probeCountMult = GetProbeCountMultiplier();

        var p = GetPlanet();
        var state = (gameStateHolder != null) ? gameStateHolder.state : null;

        if (p != null)
        {
            if (metalText) metalText.text = $"Metal: {NumberFormatter.Format(System.Math.Floor(p.metal))}";
            if (crystalText) crystalText.text = $"Crystal: {NumberFormatter.Format(System.Math.Floor(p.crystal))}";
            if (gasText) gasText.text = $"Gas: {NumberFormatter.Format(System.Math.Floor(p.gas))}";
        }

        if (probesText && state != null)
        {
            int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.Probe);
            int busy = GameStateQueries.GetBusyShips(state, ShipType.Probe);
            int total = stationed + busy;

            probesText.text = $"Probes: {stationed}/{total} (busy {busy}) ({probeCountMult:0.##}x)";
        }

        if (state != null)
        {
            int scStationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.SmallCargo);
            int scBusy = GameStateQueries.GetBusyShips(state, ShipType.SmallCargo);

            int lcStationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.LargeCargo);
            int lcBusy = GameStateQueries.GetBusyShips(state, ShipType.LargeCargo);

            if (smallCargoText) smallCargoText.text = $"Small Cargo: {scStationed} (busy {scBusy})";
            if (largeCargoText) largeCargoText.text = $"Large Cargo: {lcStationed} (busy {lcBusy})";
        }

        if (smallCargoCostText) smallCargoCostText.text = $"Cost: {smallCargoCostMetal} Metal";
        if (largeCargoCostText) largeCargoCostText.text = $"Cost: {largeCargoCostMetal} Metal";

        if (buildSmallCargoButton)
            buildSmallCargoButton.interactable = MetalFloor() >= smallCargoCostMetal;

        if (buildLargeCargoButton)
            buildLargeCargoButton.interactable = MetalFloor() >= largeCargoCostMetal;

        if (metalRateText) metalRateText.text = $"Metal/s: {metalPerSec:0.##}";
        if (crystalRateText) crystalRateText.text = $"Crystal/s: {crystalPerSec:0.##}";
        if (gasRateText) gasRateText.text = $"Gas/s: {gasPerSec:0.##}";

        // ----------------------------
        // Building scaled costs + times
        // ----------------------------
        int refineryLevel = (p != null) ? p.metalRefineryLevel : 0;
        int crystalLevel = (p != null) ? p.crystalMineLevel : 0;
        int gasLevel = (p != null) ? p.gasExtractorLevel : 0;

        double refineryMetalCost = BuildingBalance.GetMetalCost(BuildingType.MetalRefinery, refineryLevel);
        double refineryCrystalCost = BuildingBalance.GetCrystalCost(BuildingType.MetalRefinery, refineryLevel);
        double refineryGasCost = BuildingBalance.GetGasCost(BuildingType.MetalRefinery, refineryLevel);
        double refineryBuildTime = BuildingBalance.GetBuildTimeSeconds(BuildingType.MetalRefinery, refineryLevel);

        double crystalMineMetalCost = BuildingBalance.GetMetalCost(BuildingType.CrystalMine, crystalLevel);
        double crystalMineCrystalCost = BuildingBalance.GetCrystalCost(BuildingType.CrystalMine, crystalLevel);
        double crystalMineGasCost = BuildingBalance.GetGasCost(BuildingType.CrystalMine, crystalLevel);
        double crystalMineBuildTime = BuildingBalance.GetBuildTimeSeconds(BuildingType.CrystalMine, crystalLevel);

        double gasExtractorMetalCost = BuildingBalance.GetMetalCost(BuildingType.GasExtractor, gasLevel);
        double gasExtractorCrystalCost = BuildingBalance.GetCrystalCost(BuildingType.GasExtractor, gasLevel);
        double gasExtractorGasCost = BuildingBalance.GetGasCost(BuildingType.GasExtractor, gasLevel);
        double gasExtractorBuildTime = BuildingBalance.GetBuildTimeSeconds(BuildingType.GasExtractor, gasLevel);

        if (refineryCostText)
        {
            refineryCostText.text =
                $"Cost: {NumberFormatter.Format(refineryMetalCost)}M / " +
                $"{NumberFormatter.Format(refineryCrystalCost)}C / " +
                $"{NumberFormatter.Format(refineryGasCost)}G\n" +
                $"Time: {TimeFormatUtility.FormatDuration(refineryBuildTime)}";
        }

        if (crystalMineCostText)
        {
            crystalMineCostText.text =
                $"Cost: {NumberFormatter.Format(crystalMineMetalCost)}M / " +
                $"{NumberFormatter.Format(crystalMineCrystalCost)}C / " +
                $"{NumberFormatter.Format(crystalMineGasCost)}G\n" +
                $"Time: {TimeFormatUtility.FormatDuration(crystalMineBuildTime)}";
        }

        if (gasExtractorCostText)
        {
            gasExtractorCostText.text =
                $"Cost: {NumberFormatter.Format(gasExtractorMetalCost)}M / " +
                $"{NumberFormatter.Format(gasExtractorCrystalCost)}C / " +
                $"{NumberFormatter.Format(gasExtractorGasCost)}G\n" +
                $"Time: {TimeFormatUtility.FormatDuration(gasExtractorBuildTime)}";
        }

        if (probeCostText)
            probeCostText.text = $"Cost: {probeCostMetal}M/{probeCostCrystal}C/{probeCostGas}G";

        if (refineryLevelText)
            refineryLevelText.text = $"Level {refineryLevel} → {refineryLevel + 1}";

        if (crystalLevelText)
            crystalLevelText.text = $"Level {crystalLevel} → {crystalLevel + 1}";

        if (gasLevelText)
            gasLevelText.text = $"Level {gasLevel} → {gasLevel + 1}";

        if (probeLevelText)
        {
            int totalProbes = GetTotalProbesFromGameState();
            probeLevelText.text = $"Level {totalProbes}";
        }

        // -----------------------------------
        // Building button interactable states
        // -----------------------------------
        bool canAffordRefinery =
            p != null &&
            p.metal >= refineryMetalCost &&
            p.crystal >= refineryCrystalCost &&
            p.gas >= refineryGasCost;

        bool canAffordCrystalMine =
            p != null &&
            p.metal >= crystalMineMetalCost &&
            p.crystal >= crystalMineCrystalCost &&
            p.gas >= crystalMineGasCost;

        bool canAffordGasExtractor =
            p != null &&
            p.metal >= gasExtractorMetalCost &&
            p.crystal >= gasExtractorCrystalCost &&
            p.gas >= gasExtractorGasCost;

        bool buildingBusy = p != null && p.activeBuildingUpgrade != null;

        if (buildRefineryButton)
            buildRefineryButton.interactable = canAffordRefinery && !buildingBusy;

        if (buildCrystalMineButton)
            buildCrystalMineButton.interactable = canAffordCrystalMine && !buildingBusy;

        if (buildGasExtractorButton)
            buildGasExtractorButton.interactable = canAffordGasExtractor && !buildingBusy;

        if (buildProbeButton)
        {
            buildProbeButton.interactable =
                MetalFloor() >= probeCostMetal &&
                CrystalFloor() >= probeCostCrystal &&
                GasFloor() >= probeCostGas;
        }

        // -----------------------
        // Active upgrade status UI
        // -----------------------
        if (upgradeStatusText)
    {
        bool showUpgradeText = false;

        if (p != null && p.activeBuildingUpgrade != null)
        {
            double remaining = BuildingUpgradeSystem.GetRemainingTime(p);

            if (remaining > 0)
            {
                showUpgradeText = true;
                upgradeStatusText.gameObject.SetActive(true);
                upgradeStatusText.color = Color.yellow;
                upgradeStatusText.text =
                    $"Upgrading {GetBuildingDisplayName(p.activeBuildingUpgrade.buildingType)}\n" +
                    $"Remaining: {TimeFormatUtility.FormatDuration(remaining)}";
            }
        }

        if (!showUpgradeText)
        {
            upgradeStatusText.text = "";
            upgradeStatusText.gameObject.SetActive(false);
        }
    }

        RefreshBasicFighterRow();
        RefreshCargoRows();
    }

    // Building Methods
    public void TryBuildRefinery()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (BuildingUpgradeSystem.TryStartUpgrade(gameStateHolder.state, planetId, BuildingType.MetalRefinery, out string error))
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.green;
                actionStatusText.text = "Refinery upgrade started";
            }
        }
        else
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.red;
                actionStatusText.text = error;
            }
        }

        UpdateUI();
    }

    public void TryBuildCrystalMine()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (BuildingUpgradeSystem.TryStartUpgrade(gameStateHolder.state, planetId, BuildingType.CrystalMine, out string error))
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.green;
                actionStatusText.text = "Crystal Mine upgrade started";
            }
        }
        else
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.red;
                actionStatusText.text = error;
            }
        }

        UpdateUI();
    }

    public void TryBuildGasExtractor()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (BuildingUpgradeSystem.TryStartUpgrade(gameStateHolder.state, planetId, BuildingType.GasExtractor, out string error))
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.green;
                actionStatusText.text = "Gas Extractor upgrade started";
            }
        }
        else
        {
            if (actionStatusText)
            {
                actionStatusText.color = Color.red;
                actionStatusText.text = error;
            }
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
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = "Not enough resources";
            }
            return;
        }

        p.metal -= probeCostMetal;
        p.crystal -= probeCostCrystal;
        p.gas -= probeCostGas;

        QueueShipBuild(p, ShipType.Probe);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = "Probe queued, 5s";
        }

        UpdateUI();
    }

    // Fleet
    public void OpenFleet()
    {
        CloseAllMenus();
        if (fleetPanel) fleetPanel.SetActive(true);
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
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = "Not enough Metal for Basic Fighter";
            }
            return;
        }

        p.metal -= basicFighterCostMetal;

        QueueShipBuild(p, ShipType.BasicFighter);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = "Basic Fighter queued, 5s";
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
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = "Not enough Metal for Small Cargo";
            }
            return;
        }

        p.metal -= smallCargoCostMetal;
        QueueShipBuild(p, ShipType.SmallCargo);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = "Small Cargo queued, 5s";
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
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = "Not enough Metal for Large Cargo";
            }
            return;
        }

        p.metal -= largeCargoCostMetal;
        QueueShipBuild(p, ShipType.LargeCargo);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = "Large Cargo queued, 5s";
        }

        UpdateUI();
        RefreshCargoRows();
    }

    void RefreshBasicFighterRow()
    {
        if (basicFighterCostText)
            basicFighterCostText.text = $"Cost: {basicFighterCostMetal} Metal";

        if (gameStateHolder != null && gameStateHolder.state != null)
        {
            var state = gameStateHolder.state;
            int stationed = GameStateQueries.GetStationedShips(state, planetId, ShipType.BasicFighter);
            int busy = GameStateQueries.GetBusyShips(state, ShipType.BasicFighter);
            int total = stationed + busy;

            if (basicFighterOwnedText)
                basicFighterOwnedText.text = $"Owned: {stationed}";
        }

        if (buildBasicFighterButton)
            buildBasicFighterButton.interactable = MetalFloor() >= basicFighterCostMetal;
    }                                   

    void RefreshCargoRows()
    {
        var p = GetPlanet();
        if (p == null) return;

        int sc = p.GetStationed(ShipType.SmallCargo);
        int lc = p.GetStationed(ShipType.LargeCargo);

        if (smallCargoOwnedText) smallCargoOwnedText.text = $"Owned: {sc}";
        if (largeCargoOwnedText) largeCargoOwnedText.text = $"Owned: {lc}";
    }

    public void OpenBuild()
    {
        CloseAllMenus();
        if (buildPanel) buildPanel.SetActive(true);
    }

    void CloseAllMenus()
    {
        if (buildPanel) buildPanel.SetActive(false);
        if (fleetPanel) fleetPanel.SetActive(false);
    }

    public void CloseBuild() => CloseAllMenus();

        double GetMetalPerSecond()
    {
        var p = GetPlanet();
        if (p == null) return 0;

        double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
        double buildingMult = planetCollectionMult * globalEconomyMult;

        int totalProbes = GetTotalProbesFromGameState();
        double probeIncome = (totalProbes * 0.2) * probeMult;

        double buildingIncome = (p.metalRefineryLevel * 2.0) * buildingMult;

        return probeIncome + buildingIncome;
    }

    double GetCrystalPerSecond()
    {
        var p = GetPlanet();
        if (p == null) return 0;

        double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
        double buildingMult = planetCollectionMult * globalEconomyMult;

        int totalProbes = GetTotalProbesFromGameState();
        double probeIncome = (totalProbes * 0.1) * probeMult;

        double buildingIncome = (p.crystalMineLevel * 1.5) * buildingMult;

        return probeIncome + buildingIncome;
    }

    double GetGasPerSecond()
    {
        var p = GetPlanet();
        if (p == null) return 0;

        double probeMult = GetProbeCountMultiplier() * probeEfficiencyMult * globalEconomyMult;
        double buildingMult = planetCollectionMult * globalEconomyMult;

        int totalProbes = GetTotalProbesFromGameState();
        double probeIncome = (totalProbes * 0.05) * probeMult;

        double buildingIncome = (p.gasExtractorLevel * 1.0) * buildingMult;

        return probeIncome + buildingIncome;
    }

    private int GetTotalShipsFromGameState(ShipType type)
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return 0;

        var state = gameStateHolder.state;
        int stationed = GameStateQueries.GetStationedShips(state, planetId, type);
        int busy = GameStateQueries.GetBusyShips(state, type);
        return stationed + busy;
    }

    // Display Name for Upgrading Buildings
    private string GetBuildingDisplayName(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.MetalRefinery: return "Metal Refinery";
            case BuildingType.CrystalMine: return "Crystal Mine";
            case BuildingType.GasExtractor: return "Gas Extractor";
            default: return type.ToString();
        }
    }

    // FORCE SHIP BUTTON
    #if UNITY_EDITOR
    public void Debug_ForceFoundShipsNextExpedition()
    {
        ExpeditionResolver.Debug_ForceFoundShipsNext();
    }
    #endif

        double GetProbeCountMultiplier()
    {
        int totalProbes = GetTotalProbesFromGameState();

        int extra = Mathf.Max(0, totalProbes - startingProbes);
        double mult = 1.0 + (extra * bonusPerExtraProbe);

        return System.Math.Min(mult, maxProbeMultiplier);
    }

    private const double ShipBuildTimeSeconds = 5.0;

    private void QueueShipBuild(PlanetState p, ShipType shipType)
    {
        if (p == null) return;

        double startTime = Time.time;

        if (p.shipQueue != null && p.shipQueue.Count > 0)
        {
            double lastFinish = p.shipQueue[p.shipQueue.Count - 1].finishTime;
            startTime = System.Math.Max(Time.time, lastFinish);
        }

        if (p.shipQueue == null)
            p.shipQueue = new System.Collections.Generic.List<ShipQueueItem>();

        p.shipQueue.Add(new ShipQueueItem
        {
            shipType = shipType,
            finishTime = startTime + ShipBuildTimeSeconds
        });
    }

    private void TickShipQueue(PlanetState p)
    {
        if (p == null || p.shipQueue == null || p.shipQueue.Count == 0)
            return;

        while (p.shipQueue.Count > 0 && Time.time >= p.shipQueue[0].finishTime)
        {
            ShipQueueItem finished = p.shipQueue[0];
            p.shipQueue.RemoveAt(0);

            p.AddStationed(finished.shipType, 1);

            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.green;
                actionStatusText.text = $"{GetShipDisplayName(finished.shipType)} built";
            }
        }
    }

    private string GetShipDisplayName(ShipType type)
    {
        switch (type)
        {
            case ShipType.Probe: return "Probe";
            case ShipType.SmallCargo: return "Small Cargo";
            case ShipType.LargeCargo: return "Large Cargo";
            case ShipType.BasicFighter: return "Basic Fighter";
            default: return type.ToString();
        }
    }

    // Cheat Button
    #if UNITY_EDITOR
    public void DebugAddResources()
    {
        var p = GetPlanet();
        if (p == null) return;

        // +50k resources (or whatever debugAddAmount is)
        p.metal += debugAddAmount;
        p.crystal += debugAddAmount;
        p.gas += debugAddAmount;

        // +100 ships each
        p.AddStationed(ShipType.Probe, 100);
        p.AddStationed(ShipType.SmallCargo, 100);
        p.AddStationed(ShipType.LargeCargo, 100);
        p.AddStationed(ShipType.BasicFighter, 100);

        if (actionStatusText)
        {
            actionStatusText.color = Color.yellow;
            actionStatusText.text =
                $"Added {debugAddAmount} of each resource and +100 Probe/SC/LC/BF (DEBUG)";
        }

        UpdateUI();
    }
    #endif

}