using UnityEngine;

public class BuildMenuController : MonoBehaviour
{

    [SerializeField] private GameStateHolder gameStateHolder;

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

    public void UpgradeExpeditionSlots()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return;

        gameStateHolder.state.expeditionSlotsUnlocked += 1;

        Debug.Log("Expedition slots increased to: " + gameStateHolder.state.expeditionSlotsUnlocked);
    }
}