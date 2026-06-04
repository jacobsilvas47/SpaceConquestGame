using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingQueueRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private Image progressFill;
    [SerializeField] private Button cancelButton;

    private int queueIndex;
    private System.Action<int> onCancelClicked;

    public void Bind(BuildingUpgradeJob job, GameState state, int index, System.Action<int> cancelCallback)
    {
        queueIndex = index;
        onCancelClicked = cancelCallback;

        if (job == null || state == null) return;

        string buildingName = GetBuildingDisplayName(job.buildingType);

        if (nameText)
            nameText.text = $"{buildingName}  -  Lv {job.targetLevel}";

        double remaining = job.started
            ? System.Math.Max(0, job.completeTimeUtc - state.gameTime)
            : job.durationSeconds;

        if (timeText)
            timeText.text = $"Time: {TimeFormatUtility.FormatDuration(remaining)}";

        float progress = 0f;

        if (job.started && job.durationSeconds > 0)
        {
            double elapsed = state.gameTime - job.startTimeUtc;
            progress = Mathf.Clamp01((float)(elapsed / job.durationSeconds));
        }

        if (progressFill)
            progressFill.fillAmount = progress;

        if (percentText)
            percentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";

        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => onCancelClicked?.Invoke(queueIndex));
        }
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
}