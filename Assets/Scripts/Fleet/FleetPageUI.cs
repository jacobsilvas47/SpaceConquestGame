using UnityEngine;

public class FleetPageUI : MonoBehaviour
{
    [SerializeField] private GameObject fleetOverviewPanel;
    [SerializeField] private GameObject fleetBuildPanel;

    public void OpenFleetBuildPanel()
    {
        if (fleetOverviewPanel != null)
            fleetOverviewPanel.SetActive(false);

        if (fleetBuildPanel != null)
            fleetBuildPanel.SetActive(true);
    }

    public void CloseFleetBuildPanel()
    {
        if (fleetBuildPanel != null)
            fleetBuildPanel.SetActive(false);

        if (fleetOverviewPanel != null)
            fleetOverviewPanel.SetActive(true);
    }
}