using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Tutorial
{
    public abstract class UITutorialStep : TutorialStep
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        protected async UniTask EnableUI()
        {
            await _canvasGroup.DOFade(1f, 0.5f).From(0f).SetLink(_canvasGroup.gameObject).AsyncWaitForCompletion();
        }

        protected async UniTask DisableUI()
        {
            await _canvasGroup.DOFade(0f, 0.5f).From(1f).SetLink(_canvasGroup.gameObject).AsyncWaitForCompletion();
        }
    }
}