using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tutorial
{
    public abstract class TutorialStep : MonoBehaviour
    {
        private bool _isComplete;
        
        public bool IsComplete => _isComplete;
        
        public abstract UniTask StartStep();
        public abstract UniTask EndStep();
        
        protected void CompleteStep() => _isComplete = true;
    }
}