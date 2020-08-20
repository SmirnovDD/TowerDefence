using Pathfinding;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BuildMode : MonoSingleton<BuildMode>
{
    [SerializeField] private KeyCode BuildModeKey;
    public PathfindingGrid CurrentBuildGrid;
    [ReadOnly] public bool modeActive;
    public UnityEvent DestroyPlacementObj;
    public UnityEvent BuildModeActivated;

    private void Update()
    {
        if(Input.GetKeyDown(BuildModeKey))
        {
            SwitchBuildingMode();
        }
    }
    public void SwitchBuildingMode()
    {
        modeActive = !modeActive;

        if(modeActive)
        {
            //CurrentBuildGrid.NodesHolder.gameObject.SetActive(true);//TOGGLE
            BuildModeActivated?.Invoke();
        }
        else
        {
            //CurrentBuildGrid.NodesHolder.gameObject.SetActive(false);
            DestroyPlacementObj?.Invoke();
        }
    }

    public void TurnOffBuildMode()
    {
        if (modeActive == false)
            return;
        else
            SwitchBuildingMode();
    }

    public void TurnOnBuildMode()
    {
        if (modeActive)
            return;
        else
            SwitchBuildingMode();
    }
}
