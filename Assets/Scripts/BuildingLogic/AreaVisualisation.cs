using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AreaVisualisation : MonoBehaviour
{
    public void ActivateVisualisation(GameObject draggable)
    {
        List<AreaScanerController> areaScanerControllers = GetAllControlledAreaScanerControllers(draggable);

        for (int i = 0; i < areaScanerControllers.Count; i++)
        {
            areaScanerControllers[i].EnableVisualisation();
        }
    }

    public void DeactivateVisualisation(GameObject draggable)
    {
        List<AreaScanerController> areaScanerControllers = GetAllControlledAreaScanerControllers(draggable);
        
        for (int i = 0; i < areaScanerControllers.Count; i++)
        {
            areaScanerControllers[i].DisableVisualisation();
        }
    }

    private List<AreaScanerController> GetAllControlledAreaScanerControllers(GameObject mainObject)
    {
        List<AreaScanerController> areaScanerControllers = mainObject.GetComponentsInChildren<AreaScanerController>().ToList();

        if (mainObject.TryGetComponent(out AreaManager areaManager))
        {
            areaScanerControllers.AddRange(areaManager.ControlledScanerControllers);

            areaScanerControllers = areaScanerControllers.Distinct().ToList();
        }
        
        return areaScanerControllers;
    }
}