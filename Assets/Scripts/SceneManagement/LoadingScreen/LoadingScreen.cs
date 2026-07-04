using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using CuroLoading;

namespace CuroSceneManagement
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        private static LoadingScreen _instance;
        
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("LoadingIconAnimation")] 
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private RectTransform _loadingIconTransform;
        [SerializeField] private float _loopDuration;
        
        private LoadingScreenInitializationConfig _initializationConfig;
        
        public static LoadingScreen Instance => _instance;
        
        public void Initialize(LoadingScreenInitializationConfig initializationConfig)
        {
            _initializationConfig = initializationConfig;
            _instance = this;

            _canvasGroup.gameObject.SetActive(false);
            _loadingCanvasGroup.gameObject.SetActive(false);
        }

        public async UniTask Enable()
        {
            _canvasGroup.gameObject.SetActive(true);
            
            _canvasGroup.DOKill();
            await _canvasGroup.DOFade(1f, _initializationConfig.AnimationDuration).SetUpdate(true).SetLink(gameObject).SetEase(_initializationConfig.AnimationCurve).AsyncWaitForCompletion();
        }
        
        public async UniTask Disable()
        {
            _canvasGroup.DOKill();
            await _canvasGroup.DOFade(0f, _initializationConfig.AnimationDuration).SetUpdate(true).SetLink(gameObject).SetEase(_initializationConfig.AnimationCurve).AsyncWaitForCompletion();
            
            _canvasGroup.gameObject.SetActive(false);
        }
        
        public async UniTask StartPlayingLoadingIconAnimation()
        {
            _loadingCanvasGroup.gameObject.SetActive(true);
            
            _loadingIconTransform.DOKill();
            _loadingIconTransform.transform.DORotate(new Vector3(0f, 0f, -360f), _loopDuration, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetLink(gameObject).SetEase(Ease.Linear).SetUpdate(true);
            
            _loadingCanvasGroup.DOKill();
            await _loadingCanvasGroup.DOFade(1f, _initializationConfig.AnimationDuration).SetUpdate(true).SetLink(gameObject).SetEase(_initializationConfig.AnimationCurve).AsyncWaitForCompletion();
        }

        public async UniTask StopPlayingLoadingIconAnimation()
        {
            _loadingIconTransform.DOKill();
            _loadingIconTransform.transform.DORotate(new Vector3(0f, 0f, -360f), _loopDuration, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetLink(gameObject).SetEase(Ease.Linear).SetUpdate(true);
            
            _loadingCanvasGroup.DOKill();
            await _loadingCanvasGroup.DOFade(0f, _initializationConfig.AnimationDuration).SetUpdate(true).SetLink(gameObject).SetEase(_initializationConfig.AnimationCurve).AsyncWaitForCompletion();
            
            _loadingIconTransform.transform.DOKill();
            
            _loadingCanvasGroup.gameObject.SetActive(false);
        }
    }
}