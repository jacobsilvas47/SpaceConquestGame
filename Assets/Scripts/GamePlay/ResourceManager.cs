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

    private double MetalFloor() => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().metal);
    private double CrystalFloor() => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().crystal);
    private double GasFloor() => GetPlanet() == null ? 0 : System.Math.Floor(GetPlanet().gas);

    [Header("DEBUG CHEATS (Editor Only)")]
    public double debugAddAmount = 50000;

    [Header("Current Resources (Debug Mirror of Planet, do not spend from these)")]
    [SerializeField] private double metal;
    [SerializeField] private double crystal;
    [SerializeField] private double gas;

    [Header("Ship Costs (metal only, for now)")]
    public int smallCargoCostMetal = 200;
    public int largeCargoCostMetal = 600;

    [Header("Ship Cargo Capacity")]
    public int smallCargoCapacity = 500;
    public int largeCargoCapacity = 2000;

    [Header("Costs")]
    public int refineryCostMetal = 50;
    public int crystalMineCostMetal = 75;
    public int gasExtractorCostMetal = 100;
    public int probeCostMetal = 100;
    public int probeCostCrystal = 100;
    public int probeCostGas = 100;

    [Header("Level")]
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
    public TextMeshProUGUI actionStatusText;
    public TextMeshProUGUI upgradeStatusText;
    public TextMeshProUGUI shipQueueText;

    [Header("Income UI Text (optional)")]
    public TextMeshProUGUI metalRateText;
    public TextMeshProUGUI crystalRateText;
    public TextMeshProUGUI gasRateText;

    [Header("Probe Multiplier Tuning")]
    public int startingProbes = 5;
    public double bonusPerExtraProbe = 0.02;
    public double maxProbeMultiplier = 5.0;

    [Header("Fleet UI")]
    public GameObject fleetPanel;
    public UnityEngine.UI.Button fleetButton;
    public UnityEngine.UI.Button closeFleetButton;

    public UnityEngine.UI.Button buildBasicFighterButton;

    public TMPro.TextMeshProUGUI basicFighterOwnedText;
    public TMPro.TextMeshProUGUI basicFighterCostText;

    [Header("Fleet Quantity Inputs")]
    public TMP_InputField basicFighterInput;
    public TMP_InputField smallCargoInput;
    public TMP_InputField largeCargoInput;
    public TMP_InputField probeInput;

    [Header("Fleet Batch Build UI")]
    public Button buildAllShipsButton;

    [Header("Fleet")]
    public int basicFighterCostMetal = 50;

    [Header("Menu Panels")]
    public GameObject buildPanel;

    [Header("Menu Buttons")]
    public Button buildMenuButton;
    public Button closeBuildButton;

    void Start()
    {
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
            BuildingUpgradeSystem.TickPlanet(p);
            TickShipQueue(p);

            p.metal += metalPerSec * Time.deltaTime;
            p.crystal += crystalPerSec * Time.deltaTime;
            p.gas += gasPerSec * Time.deltaTime;

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
        return Mathf.Max(0, stationed);
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

        if (smallCargoCostText) smallCargoCostText.text = $"Cost: {NumberFormatter.Format(smallCargoCostMetal)} Metal";
        if (largeCargoCostText) largeCargoCostText.text = $"Cost: {NumberFormatter.Format(largeCargoCostMetal)} Metal";

        if (buildSmallCargoButton)
            buildSmallCargoButton.interactable = MetalFloor() >= smallCargoCostMetal;

        if (buildLargeCargoButton)
            buildLargeCargoButton.interactable = MetalFloor() >= largeCargoCostMetal;

        if (metalRateText) metalRateText.text = $"Metal/s: {metalPerSec:0.##}";
        if (crystalRateText) crystalRateText.text = $"Crystal/s: {crystalPerSec:0.##}";
        if (gasRateText) gasRateText.text = $"Gas/s: {gasPerSec:0.##}";

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
        {
            probeCostText.text =
                $"Cost: {NumberFormatter.Format(probeCostMetal)}M/" +
                $"{NumberFormatter.Format(probeCostCrystal)}C/" +
                $"{NumberFormatter.Format(probeCostGas)}G";
        }

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

    private void ResetInput(TMP_InputField inputField)
    {
        if (inputField != null)
            inputField.text = "1";
    }

    private void ResetBatchInputs()
    {
        if (basicFighterInput) basicFighterInput.text = "";
        if (smallCargoInput) smallCargoInput.text = "";
        if (largeCargoInput) largeCargoInput.text = "";
        if (probeInput) probeInput.text = "";
    }

    public void OpenFleet()
    {
        CloseAllMenus();
        if (fleetPanel) fleetPanel.SetActive(true);
    }

    public void CloseFleet() => CloseAllMenus();

    private int ReadBuildAmount(TMP_InputField inputField)
    {
        if (inputField == null)
            return 1;

        if (string.IsNullOrWhiteSpace(inputField.text))
            return 1;

        if (!int.TryParse(inputField.text, out int amount))
            return 1;

        return Mathf.Max(1, amount);
    }

    private int ReadBatchAmount(TMP_InputField inputField)
    {
        if (inputField == null)
            return 0;

        if (string.IsNullOrWhiteSpace(inputField.text))
            return 0;

        if (!int.TryParse(inputField.text, out int amount))
            return 0;

        return Mathf.Max(0, amount);
    }

    private bool MeetsShipRequirements(ShipType shipType, out string error)
    {
        error = null;

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            error = "Game state missing.";
            return false;
        }

        var ship = ShipDatabase.Get(shipType);
        if (ship == null)
        {
            error = $"No ship data found for {shipType}.";
            return false;
        }

        if (!RequirementUtility.MeetsRequirements(gameStateHolder.state, planetId, ship.requirements))
        {
            error = GetRequirementText(ship);
            return false;
        }

        return true;
    }

    private string GetRequirementText(ShipData ship)
    {
        if (ship == null || ship.requirements == null || ship.requirements.Count == 0)
            return "Requirements not met.";

        var parts = new System.Collections.Generic.List<string>();

        foreach (var req in ship.requirements)
        {
            parts.Add($"{GetRequirementDisplayName(req.type)} Level {req.level}");
        }

        return "Requires: " + string.Join(", ", parts);
    }

    private string GetRequirementDisplayName(RequirementType type)
    {
        switch (type)
        {
            case RequirementType.OrbitalShipworks: return "Orbital Shipworks";
            case RequirementType.MetalRefinery: return "Metal Refinery";
            case RequirementType.CrystalMine: return "Crystal Mine";
            case RequirementType.GasExtractor: return "Gas Extractor";
            case RequirementType.Barracks: return "Barracks";
            case RequirementType.ResearchLab: return "Research Lab";
            case RequirementType.LaserTech: return "Laser Tech";
            case RequirementType.ArmorTech: return "Armor Tech";
            case RequirementType.EngineTech: return "Engine Tech";
            default: return type.ToString();
        }
    }

    public bool TryBuildShip(ShipType shipType, int amount, out string error)
    {
        error = null;

        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            error = "Game state missing.";
            return false;
        }

        if (amount <= 0)
        {
            error = "Enter an amount greater than 0.";
            return false;
        }

        if (!MeetsShipRequirements(shipType, out error))
        {
            UpdateUI();
            return false;
        }

        for (int i = 0; i < amount; i++)
        {
            if (!ShipyardQueueSystem.TryEnqueueShip(gameStateHolder.state, planetId, shipType, out error))
            {
                UpdateUI();
                return false;
            }
        }

        UpdateUI();
        return true;
    }

    public void TryBuildBasicFighter()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (!MeetsShipRequirements(ShipType.BasicFighter, out string requirementError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = requirementError;
        }
        return;
    }

        int amount = ReadBuildAmount(basicFighterInput);
        int totalMetalCost = basicFighterCostMetal * amount;

        if (MetalFloor() < totalMetalCost)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = $"Not enough Metal for {amount} Basic Fighter";
            }
            return;
        }

        p.metal -= totalMetalCost;

        QueueShipBuild(p, ShipType.BasicFighter, amount);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = $"{amount} Basic Fighter queued";
        }

        ResetInput(basicFighterInput);
        UpdateUI();
        RefreshBasicFighterRow();
    }

    public void TryBuildSmallCargo()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (!MeetsShipRequirements(ShipType.SmallCargo, out string requirementError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = requirementError;
        }
        return;
    }

        int amount = ReadBuildAmount(smallCargoInput);
        int totalMetalCost = smallCargoCostMetal * amount;

        if (MetalFloor() < totalMetalCost)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = $"Not enough Metal for {amount} Small Cargo";
            }
            return;
        }

        p.metal -= totalMetalCost;
        QueueShipBuild(p, ShipType.SmallCargo, amount);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = $"{amount} Small Cargo queued";
        }

        ResetInput(smallCargoInput);
        UpdateUI();
        RefreshCargoRows();
    }

    public void TryBuildLargeCargo()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (!MeetsShipRequirements(ShipType.LargeCargo, out string requirementError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = requirementError;
        }
        return;
    }

        int amount = ReadBuildAmount(largeCargoInput);
        int totalMetalCost = largeCargoCostMetal * amount;

        if (MetalFloor() < totalMetalCost)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = $"Not enough Metal for {amount} Large Cargo";
            }
            return;
        }

        p.metal -= totalMetalCost;
        QueueShipBuild(p, ShipType.LargeCargo, amount);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = $"{amount} Large Cargo queued";
        }

        ResetInput(largeCargoInput);
        UpdateUI();
        RefreshCargoRows();
    }

    void RefreshBasicFighterRow()
    {
        if (basicFighterCostText)
            basicFighterCostText.text = $"Cost: {NumberFormatter.Format(basicFighterCostMetal)} Metal";

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

    public void TryBuildProbe()
    {
        var p = GetPlanet();
        if (p == null) return;

        if (!MeetsShipRequirements(ShipType.Probe, out string requirementError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = requirementError;
        }
        return;
    }

        int amount = ReadBuildAmount(probeInput);

        int totalMetalCost = probeCostMetal * amount;
        int totalCrystalCost = probeCostCrystal * amount;
        int totalGasCost = probeCostGas * amount;

        if (MetalFloor() < totalMetalCost ||
            CrystalFloor() < totalCrystalCost ||
            GasFloor() < totalGasCost)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = $"Not enough resources for {amount} Probe";
            }
            return;
        }

        p.metal -= totalMetalCost;
        p.crystal -= totalCrystalCost;
        p.gas -= totalGasCost;

        QueueShipBuild(p, ShipType.Probe, amount);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text = $"{amount} Probe queued";
        }

        ResetInput(probeInput);
        UpdateUI();
    }

    public void TryBuildAllShips()
    {
        var p = GetPlanet();
        if (p == null) return;

        int basicFighters = ReadBatchAmount(basicFighterInput);
        int smallCargos = ReadBatchAmount(smallCargoInput);
        int largeCargos = ReadBatchAmount(largeCargoInput);
        int probes = ReadBatchAmount(probeInput);

        int totalRequested = basicFighters + smallCargos + largeCargos + probes;

        if (totalRequested <= 0)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = "Enter at least one ship amount";
            }
            return;
        }

        if (basicFighters > 0 && !MeetsShipRequirements(ShipType.BasicFighter, out string bfError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = bfError;
        }
        return;
    }

    if (smallCargos > 0 && !MeetsShipRequirements(ShipType.SmallCargo, out string scError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = scError;
        }
        return;
    }

    if (largeCargos > 0 && !MeetsShipRequirements(ShipType.LargeCargo, out string lcError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = lcError;
        }
        return;
    }

    if (probes > 0 && !MeetsShipRequirements(ShipType.Probe, out string probeError))
    {
        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.red;
            actionStatusText.text = probeError;
        }
        return;
    }

        int totalMetalCost =
            (basicFighters * basicFighterCostMetal) +
            (smallCargos * smallCargoCostMetal) +
            (largeCargos * largeCargoCostMetal) +
            (probes * probeCostMetal);

        int totalCrystalCost =
            probes * probeCostCrystal;

        int totalGasCost =
            probes * probeCostGas;

        if (MetalFloor() < totalMetalCost ||
            CrystalFloor() < totalCrystalCost ||
            GasFloor() < totalGasCost)
        {
            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text =
                    $"Not enough resources\nNeed {NumberFormatter.Format(totalMetalCost)}M / " +
                    $"{NumberFormatter.Format(totalCrystalCost)}C / " +
                    $"{NumberFormatter.Format(totalGasCost)}G";
            }
            return;
        }

        p.metal -= totalMetalCost;
        p.crystal -= totalCrystalCost;
        p.gas -= totalGasCost;

        if (basicFighters > 0)
            QueueShipBuild(p, ShipType.BasicFighter, basicFighters);

        if (smallCargos > 0)
            QueueShipBuild(p, ShipType.SmallCargo, smallCargos);

        if (largeCargos > 0)
            QueueShipBuild(p, ShipType.LargeCargo, largeCargos);

        if (probes > 0)
            QueueShipBuild(p, ShipType.Probe, probes);

        if (actionStatusText)
        {
            actionStatusText.gameObject.SetActive(true);
            actionStatusText.color = Color.green;
            actionStatusText.text =
                $"Queued {NumberFormatter.Format(totalRequested)} ship(s)\n" +
                $"Spent {NumberFormatter.Format(totalMetalCost)}M / " +
                $"{NumberFormatter.Format(totalCrystalCost)}C / " +
                $"{NumberFormatter.Format(totalGasCost)}G";
        }

        ResetBatchInputs();
        UpdateUI();
        RefreshBasicFighterRow();
        RefreshCargoRows();
    }

    public bool TryQueueShipBuild(ShipType shipType, int amount)
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            Debug.LogError("[ResourceManager] Game state missing.");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("[ResourceManager] Amount must be greater than 0.");
            return false;
        }

        if (!MeetsShipRequirements(shipType, out string requirementError))
        {
            Debug.LogWarning($"[ResourceManager] Cannot build {shipType}: {requirementError}");

            if (actionStatusText)
            {
                actionStatusText.gameObject.SetActive(true);
                actionStatusText.color = Color.red;
                actionStatusText.text = requirementError;
            }

            UpdateUI();
            return false;
        }

        for (int i = 0; i < amount; i++)
        {
            if (!ShipyardQueueSystem.TryEnqueueShip(gameStateHolder.state, planetId, shipType, out string error))
            {
                Debug.LogWarning($"[ResourceManager] Failed to queue {shipType}: {error}");
                UpdateUI();
                return false;
            }
        }

        Debug.Log($"[ResourceManager] Queued {amount} {shipType}(s).");
        UpdateUI();
        return true;
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

    private string GetBuildingDisplayName(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.MetalRefinery: return "Metal Refinery";
            case BuildingType.CrystalMine: return "Crystal Mine";
            case BuildingType.GasExtractor: return "Gas Extractor";
            case BuildingType.OrbitalShipworks: return "Orbital Shipworks";
            default: return type.ToString();
        }
    }

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

    private void QueueShipBuild(PlanetState p, ShipType shipType, int amount)
    {
        if (p == null || amount <= 0) return;

        if (p.shipQueue == null)
            p.shipQueue = new System.Collections.Generic.List<ShipQueueItem>();

        double startTime = Time.time;

        if (p.shipQueue.Count > 0)
        {
            double lastFinish = p.shipQueue[p.shipQueue.Count - 1].finishTime;
            startTime = System.Math.Max(Time.time, lastFinish);
        }

        for (int i = 0; i < amount; i++)
        {
            p.shipQueue.Add(new ShipQueueItem
            {
                shipType = shipType,
                finishTime = startTime + ShipBuildTimeSeconds
            });

            startTime += ShipBuildTimeSeconds;
        }
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

#if UNITY_EDITOR
    public void DebugAddResources()
    {
        var p = GetPlanet();
        if (p == null) return;

        p.metal += debugAddAmount;
        p.crystal += debugAddAmount;
        p.gas += debugAddAmount;

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