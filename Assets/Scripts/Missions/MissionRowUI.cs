using TMPro;
using UnityEngine;

public class MissionRowUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Bind(string title, string target, string status, string timer)
    {
        if (titleText != null)
            titleText.text = title;

        if (targetText != null)
            targetText.text = target;

        if (statusText != null)
            statusText.text = status;

        if (timerText != null)
            timerText.text = timer;
    }
}