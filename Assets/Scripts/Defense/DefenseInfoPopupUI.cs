using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseInfoPopupUI : MonoBehaviour
{
    public static DefenseInfoPopupUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(string title, string body)
    {
        if (panel != null)
            panel.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        if (bodyText != null)
            bodyText.text = body;
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}