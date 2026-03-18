using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AITargetRowUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI lootText;
    [SerializeField] private Button attackButton;

    private AttackTarget boundTarget;
    private System.Action<AttackTarget> onAttackClicked;

    public void Bind(AttackTarget target, System.Action<AttackTarget> attackCallback)
    {
        boundTarget = target;
        onAttackClicked = attackCallback;

        if (boundTarget == null) return;

        if (nameText != null)
            nameText.text = boundTarget.displayName;

        if (infoText != null)
            infoText.text = $"{boundTarget.factionType} , {boundTarget.tier}";

        if (lootText != null)
            lootText.text =
                $"Loot: {NumberFormatter.Format(boundTarget.metal)} M, " +
                $"{NumberFormatter.Format(boundTarget.crystal)} C, " +
                $"{NumberFormatter.Format(boundTarget.gas)} G";

        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(OnAttackClicked);
        }
    }

    private void OnAttackClicked()
    {
        if (boundTarget == null) return;
        onAttackClicked?.Invoke(boundTarget);
    }
}