using UnityEngine;

public class UIPageController : MonoBehaviour
{
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject expeditionsPage;

    public void ShowMain()
    {
        if (mainPage) mainPage.SetActive(true);
        if (expeditionsPage) expeditionsPage.SetActive(false);
    }

    public void ShowExpeditions()
    {
        if (mainPage) mainPage.SetActive(false);
        if (expeditionsPage) expeditionsPage.SetActive(true);
    }
}