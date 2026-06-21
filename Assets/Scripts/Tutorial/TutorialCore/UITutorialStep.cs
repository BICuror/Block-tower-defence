using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Tutorial
{
    public abstract class UITutorialStep : TutorialStep
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public override async UniTask StartStep()
        {
            await _canvasGroup.DOFade(1f, 0.5f).From(0f).SetLink(_canvasGroup.gameObject).From(0f).AsyncWaitForCompletion();
        }

        public override async UniTask EndStep()
        {
            await _canvasGroup.DOFade(0f, 0.5f).From(0f).SetLink(_canvasGroup.gameObject).From(1f).AsyncWaitForCompletion();
        }
    }
}