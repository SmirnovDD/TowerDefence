using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeObjectToPlaceSelector : MonoBehaviour
{
    public ObjectPlacement ObjectPlacement;
    public void SelectObjectToPlace(GameObject objectToPlace)
    {
        Debug.Log(objectToPlace.name);
        ObjectPlacement.ChangePlacementObject(objectToPlace);
    }
}
