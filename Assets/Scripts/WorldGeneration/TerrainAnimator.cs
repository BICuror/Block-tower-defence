using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;

namespace WorldGeneration
{
    public sealed class TerrainAnimator : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> _meshRenderers;
        [SerializeField] private AnimationCurve _transitionCurve;
        [SerializeField] private float _radius;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private Material _transitionMaterial;
        [SerializeField] private TileTerrainGenerator _tileTerrainGenerator;
        public Material TransitionMaterial => _transitionMaterial;

        public UnityEvent AnitmationStarted;
        public UnityEvent AnimationEnded;

        public UnityEvent<Vector3> CenterSet;
        public UnityEvent<float> RadiusSet;

        private void Awake()
        {
            _transitionMaterial = new Material(_transitionMaterial);
        }

        public void SetCenter(Vector3 position)
        {
            CenterSet.Invoke(position);
            _transitionMaterial.SetVector("Center", position);
        }
        
        private void SetRadiusToTransitionMaterial(float radius)
        {
            RadiusSet.Invoke(radius);
            _transitionMaterial.SetFloat("Distance", radius);
        }

        public void StartDisappearing(float duration)
        {
            _tileTerrainGenerator.InstantiatedTiles.ForEach(tile => tile.SetTransitionMaterial());
            
            _meshRenderers.ForEach(renderer => renderer.sharedMaterial = _transitionMaterial);
            
            AnitmationStarted.Invoke();

            DOVirtual.Float(_radius, 0, duration, SetRadiusToTransitionMaterial).SetEase(_transitionCurve);
        }

        public void StartAppearing(float duration)
        {
            _tileTerrainGenerator.InstantiatedTiles.ForEach(tile => tile.SetTransitionMaterial());
            
            _meshRenderers.ForEach(renderer => renderer.sharedMaterial = _transitionMaterial);

            DOVirtual.Float(0, _radius, duration, SetRadiusToTransitionMaterial).SetEase(_transitionCurve).OnComplete(Appear);
        }

        private void Appear()
        {
            _tileTerrainGenerator.InstantiatedTiles.ForEach(tile => tile.SetMainMaterial());
            
            _meshRenderers.ForEach(renderer => renderer.sharedMaterial = _baseMaterial);

            AnimationEnded.Invoke();
        }    
    }
}
