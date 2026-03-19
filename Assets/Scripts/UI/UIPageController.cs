using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject targetsPage;
    [SerializeField] private GameObject activeMissionsPage;
    public GameObject mainPage;
    public GameObject buildPage;
    public GameObject fleetPage;
    public GameObject expeditionsPage;
    public GameObject consolePage;
    public GameObject inventoryPage;
    public GameObject defensePage;

    public GameObject resourceHUD;

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

     public void OpenDefensePage()
    {
        HideAllPages();
        if (defensePage) defensePage.SetActive(true);
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

    public void ShowTargetsPage()
    {
        HideAllPages();
        if (targetsPage != null) targetsPage.SetActive(true);
    }

    public void ShowActiveMissionsPage()
    {
        HideAllPages();
        if (activeMissionsPage != null) activeMissionsPage.SetActive(true);
    }

    void HideAllPages()
    {
        if (mainPage) mainPage.SetActive(false);
        if (buildPage) buildPage.SetActive(false);
        if (fleetPage) fleetPage.SetActive(false);
        if (expeditionsPage) expeditionsPage.SetActive(false);
        if (consolePage) consolePage.SetActive(false);
        if (inventoryPage) inventoryPage.SetActive(false);
        if (defensePage) defensePage.SetActive(false);
        if (targetsPage) targetsPage.SetActive(false);
        if (activeMissionsPage) activeMissionsPage.SetActive(false);
    }
}