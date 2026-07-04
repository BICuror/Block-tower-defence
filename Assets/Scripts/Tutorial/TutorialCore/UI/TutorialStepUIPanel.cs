using Cysharp.Threading.Tasks;
using CuroLocalization;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace Tutorial
{
    public sealed class TutorialStepUIPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("TextFields")] 
        [SerializeField] private string _goalLabelLocKey;
        [SerializeField] private TextMeshProUGUI _titleTextField;
        [SerializeField] private TextMeshProUGUI _descriptionTextField;
        [SerializeField] private TextMeshProUGUI _goalTextField;

        [Header("ProgressBar")] 
        [SerializeField] private string _progressLabelLocKey;
        [SerializeField] private TextMeshProUGUI _progressLabelTextField;
        [SerializeField] private AnimationCurve _progressBarFillCurve;
        [SerializeField] private Slider _progressBar;
        private bool _presentProgressAsPercent;

        public void Initialize(string mainKey)
        {
            _titleTextField.text = ($"{mainKey}_header").Localize();
            _descriptionTextField.text = ($"{mainKey}_description").Localize();
            _goalTextField.text = $"{_goalLabelLocKey.Localize()} {$"{mainKey}_goal".Localize()}";
            
            _progressBar.gameObject.SetActive(false);
        }

        public void InitializeProgressBar(bool presentProgressAsPercent, int requiredValue)
        {
            _progressBar.gameObject.SetActive(true);
            _progressBar.DOKill();
            _progressBar.value = 0f;
            
            _presentProgressAsPercent = presentProgressAsPercent;

            UpdateDisplayValue(0, requiredValue);
        }

        public void SetProgress(int currentProgress, int maxProgress)
        {
            float progress = currentProgress / (float)maxProgress;
            _progressBar.DOValue(progress, 0.2f).SetEase(_progressBarFillCurve).SetLink(gameObject);
            
            UpdateDisplayValue(currentProgress, maxProgress);
        }

        private void UpdateDisplayValue(int currentProgress, int maxProgress)
        {
            if (_presentProgressAsPercent)
            {
                float progress = currentProgress / (float)maxProgress;
                
                _progressLabelTextField.text = $"{_progressLabelLocKey.Localize()} {(int)(progress * 100)}%";
            }
            else
            {
                _progressLabelTextField.text = $"{_progressLabelLocKey.Localize()} {currentProgress}/{maxProgress}";
            }
        }
        
        public async UniTask Enable()
        {
            gameObject.SetActive(true);
            
            _canvasGroup.DOKill();
            
            await _canvasGroup.DOFade(1f, 0.5f).SetLink(_canvasGroup.gameObject).AsyncWaitForCompletion();
        }

        public async UniTask Disable()
        {
            gameObject.SetActive(true);
            
            _canvasGroup.DOKill();
            
            await _canvasGroup.DOFade(0f, 0.5f).SetLink(_canvasGroup.gameObject).AsyncWaitForCompletion();
            
            gameObject.SetActive(false);
        }
    }
}