using UnityEngine.UI;
using UnityEngine;

namespace Tutorial
{
    public abstract class ProgressTutorialStep : UITutorialStep
    {
        [Header("Progress")]
        [SerializeField] private int _requiredProgress = 500;
        [SerializeField] private Slider _progressSlider;
        private int _progress = 0;

        protected void IncreaseProgress() => IncreaseProgress(1);
        
        protected void IncreaseProgress(int value)
        {
            _progress += value;
            
            _progressSlider.value = (float)_progress / _requiredProgress;
            
            if (_progress >= _requiredProgress) CompleteStep();
        }
    }   
}