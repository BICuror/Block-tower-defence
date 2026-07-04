using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace Tutorial
{
    public sealed class TutorialController : MonoBehaviour
    {
        [SerializeField] private List<TutorialStep> _steps;

        public event Action TutorialCompleted;
        
        private void Start() => StartTutorial().Forget();

        public async UniTask StartTutorial()
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                await _steps[i].StartStep();
                
                Debug.Log($"Started step {_steps[i].gameObject.name}");
                
                await UniTask.WaitUntil(() => _steps[i].IsComplete);
                
                Debug.Log($"Ended step {_steps[i].gameObject.name}");
                
                await _steps[i].EndStep();
            }
            
            TutorialCompleted?.Invoke();
        }
    }
}