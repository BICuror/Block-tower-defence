using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AreaVisualisationInspector : MonoBehaviour
{
    public void ActivateVisualisation(GameObject draggable)
    {
        List<AreaVisualisation> areaVisualisations = GetAllControlledAreaScanerControllers(draggable);

        for (int i = 0; i < areaVisualisations.Count; i++)
        {
            areaVisualisations[i].EnableVisualisation();
        }
    }

    public void DeactivateVisualisation(GameObject draggable)
    {
        List<AreaVisualisation> areaVisualisations = GetAllControlledAreaScanerControllers(draggable);
        
        for (int i = 0; i < areaVisualisations.Count; i++)
        {
            areaVisualisations[i].DisableVisualisation();
        }
    }

    private List<AreaVisualisation> GetAllControlledAreaScanerControllers(GameObject mainObject)
    {
        List<AreaVisualisation> areaVisualisations = mainObject.GetComponentsInChildren<AreaVisualisation>().ToList();

        if (mainObject.TryGetComponent(out AreaManager areaManager))
        {
            areaManager.ControlledScanerControllers.ForEach(areaScanerController =>
            {
                if (areaScanerController.HasVisualisation) areaVisualisations.Add(areaScanerController.AreaVisualisation);
            });

            areaVisualisations = areaVisualisations.Distinct().ToList();
        }
        
        return areaVisualisations;
    }
}