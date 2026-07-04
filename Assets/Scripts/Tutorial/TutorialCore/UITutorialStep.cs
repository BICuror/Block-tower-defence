using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tutorial
{
    public abstract class UITutorialStep : TutorialStep
    {
        [SerializeField] protected TutorialStepUIPanel StepUIPanel;
        [SerializeField] private string _mainLocKey;
        
        protected UniTask EnableUI()
        {
            InitializeUIPanel();
            
            return StepUIPanel.Enable();
        }
        
        protected UniTask DisableUI() => StepUIPanel.Disable();

        protected virtual void InitializeUIPanel()
        {
            StepUIPanel.Initialize(_mainLocKey);
        }
    }
}