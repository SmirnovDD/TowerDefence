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
            BuildModeActivated?.Invoke();
        }
        else
        {
            DestroyPlacementObj?.Invoke();
        }
    }

    public void TurnOffBuildMode()
    {
        if (modeActive)
            SwitchBuildingMode();
    }

    public void TurnOnBuildMode()
    {
        if (modeActive == false)
            SwitchBuildingMode();
    }
}
