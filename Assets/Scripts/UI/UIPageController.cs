using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject mainPage;
    public GameObject buildPage;
    public GameObject fleetPage;
    public GameObject expeditionsPage;
    public GameObject consolePage;
    public GameObject inventoryPage;

    public GameObject resourceHUD;

    void HideAllPages()
    {
        if (mainPage) mainPage.SetActive(false);
        if (buildPage) buildPage.SetActive(false);
        if (fleetPage) fleetPage.SetActive(false);
        if (expeditionsPage) expeditionsPage.SetActive(false);
        if (consolePage) consolePage.SetActive(false);
        if (inventoryPage) inventoryPage.SetActive(false);
    }

    void SetResourceHUDVisible(bool visible)
    {
        if (resourceHUD) resourceHUD.SetActive(visible);
    }

    public void OpenMainPage()
    {
        HideAllPages();
        if (mainPage) mainPage.SetActive(true);
        SetResourceHUDVisible(true);
    }

    public void OpenBuildPage()
    {
        HideAllPages();
        if (buildPage) buildPage.SetActive(true);
        SetResourceHUDVisible(true);
    }

    public void OpenFleetPage()
    {
        HideAllPages();
        if (fleetPage) fleetPage.SetActive(true);
        SetResourceHUDVisible(true);
    }

    public void OpenExpeditionsPage()
    {
        HideAllPages();
        if (expeditionsPage) expeditionsPage.SetActive(true);
        SetResourceHUDVisible(true);
    }

    public void OpenConsolePage()
    {
        HideAllPages();
        if (consolePage) consolePage.SetActive(true);
        SetResourceHUDVisible(false);
    }

    public void OpenInventoryPage()
    {
        HideAllPages();
        if (inventoryPage) inventoryPage.SetActive(true);
        SetResourceHUDVisible(false);
    }
}