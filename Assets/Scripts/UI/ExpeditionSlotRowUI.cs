using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionSlotRowUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI slotTitleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button selectButton;

    private string missionId;

    public void BindEmpty(int slotIndex)
    {
        missionId = null;

        if (slotTitleText) slotTitleText.text = $"Slot {slotIndex}";
        if (statusText) statusText.text = "Empty";
        if (timerText) timerText.text = "--";

        if (selectButton)
        {
            selectButton.interactable = false;
            selectButton.onClick.RemoveAllListeners();
        }
    }

    public void BindMission(int slotIndex, string missionId, string status, string timer, System.Action<string> onSelect)
    {
        this.missionId = missionId;

        if (slotTitleText) slotTitleText.text = $"Slot {slotIndex}";
        if (statusText) statusText.text = status;
        if (timerText) timerText.text = timer;

        if (selectButton)
        {
            selectButton.interactable = true;
            selectButton.onClick.RemoveAllListeners();
            if (onSelect != null)
                selectButton.onClick.AddListener(() => onSelect(this.missionId));
        }
    }
}