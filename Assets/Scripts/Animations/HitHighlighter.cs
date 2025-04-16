using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using Cashing;

namespace Combat.Animation
{
    public sealed class HitHighlighter : MonoBehaviour
    {
        [Cached] private EntityHealth _entityHealth;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private Material _defaultMaterial;
        
        public UnityEvent Highlited;
        public UnityEvent Unhiglited;
    
        private float _highlightDuration = 0.1f;
        private YieldInstruction _yieldInstruction;
    
        private void Start()
        {
            _defaultMaterial = _meshRenderer.sharedMaterial;
    
            _entityHealth.Damaged += HiglightEntity;
    
            _yieldInstruction = new WaitForSeconds(_highlightDuration);
    
            if (_highlightMaterial == null) Debug.LogError("Highlight material is missing on" + gameObject.name);
        }
    
        private void HiglightEntity()
        {
            StopAllCoroutines();
    
            _meshRenderer.sharedMaterial = _highlightMaterial;
    
            Highlited.Invoke();
    
            StartCoroutine(UnhighlightEntity());
        }
    
        private IEnumerator UnhighlightEntity()
        {
            yield return _yieldInstruction;
    
            _meshRenderer.sharedMaterial = _defaultMaterial;
    
            Unhiglited.Invoke();
        }
    }
}