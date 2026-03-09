using UnityEngine;

public class UIPageController : MonoBehaviour
{
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject buildPage;
    [SerializeField] private GameObject expeditionsPage;
    [SerializeField] private GameObject consolePage;
    [SerializeField] private GameObject inventoryPage;
    [SerializeField] private ConsolePageUI consolePageUI;

    private void Start()
    {
        ShowMain();
    }

    private void HideAll()
    {
        if (mainPage) mainPage.SetActive(false);
        if (buildPage) buildPage.SetActive(false);
        if (expeditionsPage) expeditionsPage.SetActive(false);
        if (consolePage) consolePage.SetActive(false);
        if (inventoryPage) inventoryPage.SetActive(false);
    }

    public void ShowMain()
    {
        HideAll();
        if (mainPage) mainPage.SetActive(true);
    }

    public void ShowBuild()
    {
        HideAll();
        if (buildPage) buildPage.SetActive(true);
    }

    public void ShowExpeditions()
    {
        HideAll();
        if (expeditionsPage) expeditionsPage.SetActive(true);
    }

    public void ShowConsole()
    {
        HideAll();
        if (consolePage) consolePage.SetActive(true);

        if (consolePageUI != null)
            consolePageUI.Refresh();
    }

    public void ShowInventory()
    {
        HideAll();
        if (inventoryPage) inventoryPage.SetActive(true);
    }
}