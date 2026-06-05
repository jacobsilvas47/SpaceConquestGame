using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchQueueRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private Image progressFill;
    [SerializeField] private Button cancelButton;

    private int queueIndex;
    private System.Action<int> onCancelClicked;

    public void Bind(ResearchJob job, GameState state, int index, System.Action<int> cancelCallback)
    {
        queueIndex = index;
        onCancelClicked = cancelCallback;

        UpdateDisplay(job, state);

        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => onCancelClicked?.Invoke(queueIndex));
        }
    }

    public void UpdateDisplay(ResearchJob job, GameState state)
    {
        if (job == null || state == null) return;

        if (nameText)
            nameText.text = $"{GetResearchDisplayName(job.researchType)}  -  Lv {job.targetLevel}";

        double remaining = job.started
            ? System.Math.Max(0, job.completeTime - state.gameTime)
            : job.durationSeconds;

        if (timeText)
            timeText.text = $"Time: {TimeFormatUtility.FormatDuration(remaining)}";

        float progress = 0f;

        if (job.started && job.durationSeconds > 0)
        {
            double elapsed = state.gameTime - job.startTime;
            progress = Mathf.Clamp01((float)(elapsed / job.durationSeconds));
        }

        if (progressFill)
            progressFill.fillAmount = progress;

        if (percentText)
            percentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
    }

    private string GetResearchDisplayName(ResearchType type)
    {
        switch (type)
        {
            case ResearchType.Engineering: return "Engineering";
            case ResearchType.WeaponSystems: return "Weapon Systems";
            case ResearchType.DefenseSystems: return "Defense Systems";
            case ResearchType.EngineTech: return "Engine Tech";
            default: return type.ToString();
        }
    }
}