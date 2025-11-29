using Cysharp.Threading.Tasks;
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

    public void ActivateVisualisationAsync(GameObject draggable) => ActivateVisualisation(draggable).Forget();
    private async UniTask ActivateVisualisation(GameObject draggable)
    {
        await UniTask.DelayFrame(2, delayTiming: PlayerLoopTiming.FixedUpdate);
        
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].EnableVisualisation(_visualisationDuration, _visualisationAppearCurve);
        }
    }

    public void DeactivateVisualisation(GameObject draggable)
    {
        AreaScanerController[] areaScanerControllers = draggable.GetComponentsInChildren<AreaScanerController>();

        for (int i = 0; i < areaScanerControllers.Length; i++)
        {
            areaScanerControllers[i].DisableVisualisation(_visualisationDuration, _visualisationDisappearCurve);
        }
    }
}
