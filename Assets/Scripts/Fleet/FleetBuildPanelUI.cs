using System.Collections.Generic;
using UnityEngine;

public class FleetBuildPanelUI : MonoBehaviour
{
    [Header("Ship Rows")]
    public List<ShipBuildRowUI> rows = new List<ShipBuildRowUI>();

    private ShipCategory currentCategory = ShipCategory.All;

    public void ShowAll()
    {
        ApplyCategoryFilter(ShipCategory.All);
    }

    public void ShowScoutsAndUtility()
    {
        ApplyCategoryFilter(ShipCategory.ScoutsAndUtility);
    }

    public void ShowCargoAndLogistics()
    {
        ApplyCategoryFilter(ShipCategory.CargoAndLogistics);
    }

    public void ShowColonization()
    {
        ApplyCategoryFilter(ShipCategory.Colonization);
    }

    public void ShowLightCombat()
    {
        ApplyCategoryFilter(ShipCategory.LightCombat);
    }

    public void ShowFrigates()
    {
        ApplyCategoryFilter(ShipCategory.Frigates);
    }

    public void ShowSpecialistCombat()
    {
        ApplyCategoryFilter(ShipCategory.SpecialistCombat);
    }

    public void ShowCapitalShips()
    {
        ApplyCategoryFilter(ShipCategory.CapitalShips);
    }

    private void ApplyCategoryFilter(ShipCategory category)
    {
        currentCategory = category;

        foreach (var row in rows)
        {
            if (row == null) continue;

            bool shouldShow =
                category == ShipCategory.All ||
                row.GetCategory() == category;

            row.gameObject.SetActive(shouldShow);
        }
    }
}