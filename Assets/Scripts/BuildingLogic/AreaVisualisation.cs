using UnityEngine;

public sealed class AreaVisualisation : MonoBehaviour
{
    public void ActivateVisualisation(GameObject draggable)
    {
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].EnableVisualisation();
        }
    }

    public void DeactivateVisualisation(GameObject draggable)
    {
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].DisableVisualisation();
        }
    }
}