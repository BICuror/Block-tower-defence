using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using Cashing;

namespace Combat.Animation
{
    public sealed class HitHighlighter : MonoBehaviour
    {
        private const float HIGHLIGHT_DURATION = 0.1f;
        
        [Cached] private EntityHealth _entityHealth;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private Material _defaultMaterial;
        
        public UnityEvent Highlited;
        public UnityEvent Unhiglited;
        
        private void Start()
        {
            _defaultMaterial = _meshRenderer.sharedMaterial;
            
            _entityHealth.Damaged += HiglightEntityAsync;
    
            if (_highlightMaterial == null) Debug.LogError("Highlight material is missing on" + gameObject.name);
        }

        public void SetDefaultMaterial(Material material)
        {
            _defaultMaterial = material;
            _meshRenderer.sharedMaterial = material;
            
            Unhiglited.Invoke();
        }

        private void HiglightEntityAsync() => HiglightEntity().Forget();
        
        private async UniTask HiglightEntity()
        {
            StopAllCoroutines();
    
            _meshRenderer.sharedMaterial = _highlightMaterial;
    
            Highlited.Invoke();
            
            await UniTask.WaitForSeconds(HIGHLIGHT_DURATION);
    
            _meshRenderer.sharedMaterial = _defaultMaterial;
    
            Unhiglited.Invoke();
        }

        private void OnDestroy()
        {
            _entityHealth.Damaged -= HiglightEntityAsync;
        }
    }
}