using DG.Tweening;
using UnityEngine;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float _fadeDuration;

    [SerializeField] private CanvasGroup _rotationPanel;
    [SerializeField] private CanvasGroup _zoomPanel;
    [SerializeField] private CanvasGroup _inspectionPanel;

    [Header("DragTutorial")]
    [SerializeField] private DraggableObject _tutorialDefenceBuidling;
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Transform _townhall;

    [Header("CrystalTutorial")]
    [SerializeField] private DraggableObject _tutorialCrystal;
    [SerializeField] private GameObject _crystalPlacementAreaHighlighter;

    private TutorialState _currentTutorialState;

    private Tween _fadeTween;
    private Tween _unfadeTween;

    private void Awake()
    {
        _rotationPanel.gameObject.SetActive(true);
        _rotationPanel.alpha = 0f;
        _fadeTween = _rotationPanel.DOFade(1f, _fadeDuration);
    
        Invoke("Starti", 1f);
    }   

    private void Starti() => _currentTutorialState = TutorialState.Rotation;

    public void SwitchToZoomTutorial()
    {
        if (_currentTutorialState == TutorialState.Rotation)
        {
            if (_fadeDuration != null && _fadeTween.IsComplete() == false) _fadeTween.Kill();
            if (_unfadeTween != null && _unfadeTween.IsComplete() == false) _unfadeTween.Kill();
            
            _zoomPanel.alpha = 0f;
            _zoomPanel.gameObject.SetActive(true);
            _fadeTween = _rotationPanel.DOFade(0f, _fadeDuration / 2).OnComplete(DisableRotationPanel);

            _unfadeTween = _zoomPanel.DOFade(1f, _fadeDuration * 2);

            _currentTutorialState = TutorialState.Zoom;
        }
    }    
    private void DisableRotationPanel() 
    {
        _rotationPanel.gameObject.SetActive(false);
    }
    
    public void SwitchToInspectionTutorial()
    {
        if (_currentTutorialState == TutorialState.Zoom)
        {
            if (_fadeDuration != null && _fadeTween.IsComplete() == false) _fadeTween.Kill();
            if (_unfadeTween != null && _unfadeTween.IsComplete() == false) _unfadeTween.Kill();
            
            _fadeTween = _zoomPanel.DOFade(0f, _fadeDuration / 2).OnComplete(DisableRotationPanel);
            _inspectionPanel.gameObject.SetActive(true);
            _inspectionPanel.alpha = 0f;
            _unfadeTween = _inspectionPanel.DOFade(1f, _fadeDuration * 2);

            _currentTutorialState = TutorialState.Inspection;
        }
    }
    private void DisableZoomPanel() => _zoomPanel.gameObject.SetActive(false);

    public void SwitchToDragTutorial()
    {
        if (_currentTutorialState == TutorialState.Inspection)
        {   
            _fadeTween = _inspectionPanel.DOFade(0f, _fadeDuration / 2).OnComplete(DisableInspectionPanel);

            _currentTutorialState = TutorialState.Drag;

            _draggableCreator.CreateDraggableOnRandomPosition(_tutorialDefenceBuidling, _townhall.position);
        }
    }

    private void DisableInspectionPanel() => _inspectionPanel.gameObject.SetActive(false);


    public void SwitchToCrystalTutorial()
    {
        if (_currentTutorialState == TutorialState.Drag)
        {   
            _currentTutorialState = TutorialState.Crystal;

            _draggableCreator.CreateDraggableOnRandomPosition(_tutorialCrystal, _townhall.position);

            _crystalPlacementAreaHighlighter.SetActive(true);
        }
    }

    public void SwitchToSelectionTutorial()
    {
        if (_currentTutorialState == TutorialState.Crystal)
        {   
            _currentTutorialState = TutorialState.Selection;

            _crystalPlacementAreaHighlighter.SetActive(false);
        }
    }

    private enum TutorialState
    {
        None,
        Rotation,
        Zoom,
        Inspection,
        Drag,
        Crystal,
        Selection,
        Defence
    }
}
