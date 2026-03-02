using UnityEngine;

public class BuildMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject buildList; // drag BuildList here

    public void ToggleBuildMenu()
    {
        if (!buildList) return;
        buildList.SetActive(!buildList.activeSelf);
    }

    public void OpenBuildMenu()
    {
        if (buildList) buildList.SetActive(true);
    }

    public void CloseBuildMenu()
    {
        if (buildList) buildList.SetActive(false);
    }
}