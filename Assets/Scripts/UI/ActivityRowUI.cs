using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityRowUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskDetailText;
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private Image progressFill;

    public void SetRow(string taskName, string detail, float progress01, string remainingTime)
    {
        if (taskNameText != null)
            taskNameText.text = taskName;

        if (taskDetailText != null)
            taskDetailText.text = detail;

        if (remainingTimeText != null)
            remainingTimeText.text = remainingTime;

        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(progress01);
    }

    public void SetInactive(string taskName, string detail)
    {
        SetRow(taskName, detail, 0f, "--");
    }
}