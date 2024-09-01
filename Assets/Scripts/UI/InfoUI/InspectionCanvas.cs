using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public sealed class InspectionCanvas : MonoBehaviour
{
    public UnityEvent CanvasChangedPosition;

    [Header("Animation")]
    [SerializeField] private AnimationCurve _appearAnimationCurve;
    [SerializeField] private float _appearAnimationDuration;

    [SerializeField] private AnimationCurve _disappearAnimationCurve;
    [SerializeField] private float _disappearAnimationDuration;

    [Header("Links")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private ParametersDisplayer _parameterDisplayer;
    [SerializeField] private AreaVisualisation _areaVisualisation;
    [SerializeField] private RectTransform _decriptionPanel;
    [SerializeField] private DescriptionDisplayer _descriptionDisplayer;

    private Tween _lastTween;

    public void EnableCanvas(InspectableObject inspectable)
    {   
        _descriptionDisplayer.DisplayDescription(inspectable);
        
        if (inspectable is BuildingInspectable)
        {
            _parameterDisplayer.gameObject.SetActive(true);

            _parameterDisplayer.DisplayParameters(inspectable as BuildingInspectable);

            if (inspectable.TryGetComponent<AreaManager>(out AreaManager manager))
            {
                if (inspectable.transform.parent != null && inspectable.transform.parent.TryGetComponent<SelectionOptionObject>(out SelectionOptionObject obj))
                {
                    _areaVisualisation.StopVisualisation();
                }
                else 
                {
                    _areaVisualisation.ActivatePositionedViualisation(inspectable.gameObject, inspectable.transform.position);
                }
            }
            else 
            {
                _areaVisualisation.StopVisualisation();
            }

            SetDesctiptionPanelHeight((inspectable as BuildingInspectable).GetAllParameters().Length);
        }
        else 
        {
            _parameterDisplayer.gameObject.SetActive(false);
        
            SetDesctiptionPanelHeight(0);
        }

        transform.DOKill();

        gameObject.SetActive(true);

        SetPosition(inspectable);
        StartAlphaAnimation();
    }

    private void SetDesctiptionPanelHeight(int paramsCount)
    {
        int height = 0;
        if (paramsCount > 6) height = 2;
        else if (paramsCount > 3) height = 1;

        _decriptionPanel.anchoredPosition = new Vector2(0f, -height);
    }

    private void SetPosition(InspectableObject inspectable)
    {
        float height = inspectable.GetInpectionPanelInstallationHeight();

        transform.position = inspectable.transform.position + new Vector3(0f, (height / 2), 0f);

        transform.DOMoveY(inspectable.transform.position.y + height, _appearAnimationDuration).SetEase(_appearAnimationCurve);
        
        CanvasChangedPosition.Invoke();
    }

    private void StartAlphaAnimation()
    {
        if (_lastTween != null && _lastTween.IsComplete() == false) _lastTween.Kill(); 

        _lastTween = DOVirtual.Float(0f, 1f, _appearAnimationDuration, SetAlpha).SetEase(_appearAnimationCurve);
    }

    private void SetAlpha(float value) => _canvasGroup.alpha = value;

    public void DisableCanvas()
    {
        transform.DOKill();

        _areaVisualisation.StopVisualisation();

        if (_lastTween != null && _lastTween.IsComplete() == false) _lastTween.Kill(); 

        _lastTween = DOVirtual.Float(_canvasGroup.alpha, 0f, _disappearAnimationDuration, SetAlpha).SetEase(_disappearAnimationCurve).OnComplete(() => gameObject.SetActive(false));
    }
}
