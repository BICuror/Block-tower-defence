using UnityEngine;

namespace Tutorial
{
    public abstract class ProgressTutorialStep : UITutorialStep
    {
        [Header("Progress")]
        [SerializeField] private bool _presentAsPercent;
        [SerializeField] private int _requiredProgress = 500;
        private int _progress = 0;

        protected void SetRequiredProgress(int requiredProgress) => _requiredProgress = requiredProgress;
        
        protected void IncreaseProgress() => ChangeProgress(1);
        protected void DecreaseProgress() => ChangeProgress(-1);
        
        protected void SetProgress(int value)
        {
            _progress = value;
            
            StepUIPanel.SetProgress(_progress, _requiredProgress);
            
            if (_progress >= _requiredProgress) CompleteStep();
        }
        
        private void ChangeProgress(int value) => SetProgress(_progress + value);
        
        protected override void InitializeUIPanel()
        {
            base.InitializeUIPanel();
            StepUIPanel.InitializeProgressBar(_presentAsPercent, _requiredProgress);
        }
    }   
}