using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject buildPage;
    [SerializeField] private GameObject fleetPage;
    [SerializeField] private GameObject targetsPage;
    [SerializeField] private GameObject defensePage;
    [SerializeField] private GameObject expeditionsPage;
    [SerializeField] private GameObject inventoryPage;
    [SerializeField] private GameObject consolePage;
    [SerializeField] private GameObject activeMissionsPage;
    [SerializeField] private GameObject researchPage;

    private void HideAllPages()
    {
        if (mainPage != null) mainPage.SetActive(false);
        if (buildPage != null) buildPage.SetActive(false);
        if (fleetPage != null) fleetPage.SetActive(false);
        if (targetsPage != null) targetsPage.SetActive(false);
        if (defensePage != null) defensePage.SetActive(false);
        if (expeditionsPage != null) expeditionsPage.SetActive(false);
        if (inventoryPage != null) inventoryPage.SetActive(false);
        if (consolePage != null) consolePage.SetActive(false);
        if (activeMissionsPage != null) activeMissionsPage.SetActive(false);
        if (researchPage != null) researchPage.SetActive(false);
    }

    public void OpenMainPage()
    {
        HideAllPages();
        if (mainPage != null) mainPage.SetActive(true);
    }

    public void OpenBuildPage()
    {
        HideAllPages();
        if (buildPage != null) buildPage.SetActive(true);
    }

    public void OpenFleetPage()
    {
        HideAllPages();
        if (fleetPage != null) fleetPage.SetActive(true);
    }

    public void ShowTargetsPage()
    {
        HideAllPages();
        if (targetsPage != null) targetsPage.SetActive(true);
    }

    public void OpenDefensePage()
    {
        HideAllPages();
        if (defensePage != null) defensePage.SetActive(true);
    }

    public void OpenExpeditionsPage()
    {
        HideAllPages();
        if (expeditionsPage != null) expeditionsPage.SetActive(true);
    }

    public void OpenInventoryPage()
    {
        HideAllPages();
        if (inventoryPage != null) inventoryPage.SetActive(true);
    }

    public void OpenConsolePage()
    {
        HideAllPages();
        if (consolePage != null) consolePage.SetActive(true);
    }

    public void OpenResearchPage()
    {
        HideAllPages();
        if (researchPage != null) researchPage.SetActive(true);
    }

    public void OpenActiveMissionsPage()
    {
        HideAllPages();
        if (activeMissionsPage != null) activeMissionsPage.SetActive(true);
    }
}