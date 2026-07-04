using Cysharp.Threading.Tasks;
using GameControls.Features;
using NaughtyAttributes;
using GameControls;
using DG.Tweening;
using UnityEngine;
using Zenject;

public sealed class SceneStartAnimationController : MonoBehaviour
{
    [Inject] private GameController _gameController;
    
    [Header("General")] 
    [SerializeField] private Transform _target;
    [SerializeField] private float _animationDuration;
    
    [Header("Zoom")]
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private AnimationCurve _zoomAnimationCurve;
    [SerializeField] private float _startZoom;
    [SerializeField] private float _endZoom;
    
    [Header("LocalPosition")] 
    [SerializeField] private AnimationCurve _localPositionAnimationCurve;
    [SerializeField] private Vector3 _startLocalPosition;
    [SerializeField] private Vector3 _endLocalPosition;
    
    [Header("Rotation")]
    [SerializeField] private AnimationCurve _rotationAnimationCurve;
    [SerializeField] private Vector3 _startRotation;
    [SerializeField] private Vector3 _endRotation;

    private void Start() => PlayAnimation().Forget();
    
    [Button]
    public async UniTask PlayAnimation()
    {
        _gameController.DisableFeature(ControllerFeature.CameraZoom);
    
        DOVirtual.Float(_startZoom, _endZoom, _animationDuration, (value) => _targetCamera.orthographicSize = value).SetEase(_zoomAnimationCurve).SetLink(_target.gameObject);;
        _target.DOLocalMove(_endLocalPosition, _animationDuration).SetEase(_localPositionAnimationCurve).From(_startLocalPosition).SetLink(_target.gameObject);
        await _target.DOLocalRotate(_endRotation, _animationDuration).SetEase(_rotationAnimationCurve).From(_startRotation).SetLink(_target.gameObject).AsyncWaitForCompletion();
        
        _gameController.EnableFeature(ControllerFeature.CameraZoom);
    }
}