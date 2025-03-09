using DG.Tweening;
using UnityEngine;

public sealed class AreaVisualisation : MonoBehaviour
{
    [SerializeField] private DraggableConnector _draggableConnector;

    [Header("Curves")]
    [SerializeField] private AnimationCurve _visualisationAppearCurve;
    [SerializeField] private AnimationCurve _visualisationDisappearCurve;

    [SerializeField] private AnimationCurve _inspectionDissapearCurve;
    [SerializeField] private float _inspectionDissapearDuraion;

    [Header("VisualisationSettings")]

    [SerializeField] private float _visualisationDuration;

    [SerializeField] private MeshFilter _reachAreaVisualisation;
    
    [SerializeField] private Mesh _defaultMesh;

    private Tween _currentTween;

    private void SetVisualisationScale(Vector3 scale)
    {
        _reachAreaVisualisation.transform.localScale = scale;
    }

    public void ActivatePositionedViualisation(GameObject draggable, Vector3 newPosition)
    {
        if (draggable.TryGetComponent<AreaManager>(out AreaManager manager))
        { 

            ActivateVisualisation(draggable);
            
            transform.position = newPosition;
        }
        else 
        {
            StopVisualisation();
        }
    }

    public void ActivateVisualisation(GameObject draggable)
    {
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].EnableVisualisation(_visualisationDuration, _visualisationAppearCurve);
        }
    }

    public void DisactivateVisualisation(GameObject draggable)
    {
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].DisableVisualisation(_visualisationDuration, _visualisationDisappearCurve);
        }
    }

    public void StopVisualisation()
    {
        if (_currentTween == null || _currentTween.IsPlaying()) _currentTween.Kill();

        _currentTween = DOVirtual.Vector3(_reachAreaVisualisation.transform.localScale, Vector3.zero, _inspectionDissapearDuraion, SetVisualisationScale).SetEase(_inspectionDissapearCurve).OnComplete(DisableVisualisationObject);
        
    }

    private void DisableVisualisationObject() => _reachAreaVisualisation.gameObject.SetActive(false);
}
