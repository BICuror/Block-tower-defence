using UnityEngine;

namespace WorldGeneration
{
    public sealed class EnemyBiomeDecorationMaterialChanger : MonoBehaviour
    {
        [SerializeField] private Material _transitionMaterial;

        [SerializeField] private Material _baseMaterial; 

        [SerializeField] private DecorationContainer _decorationContainer;

        [SerializeField] private TerrainAnimator _terrainAnimator;

        [SerializeField] private MeshRenderer _spawnerMesh;
        private Material _baseSpawnerMaterial;

        private void Awake()
        {
            _terrainAnimator.AnitmationStarted.AddListener(ApplyTransitionMaterial);
            _terrainAnimator.AnimationEnded.AddListener(ApplyBaseMaterial);
            _terrainAnimator.CenterSet.AddListener(SetCenterToTransitionMaterial);
            _terrainAnimator.RadiusSet.AddListener(SetRaduisToTransitionMaterial);

            _transitionMaterial = new Material(_transitionMaterial);

            _baseSpawnerMaterial = _spawnerMesh.sharedMaterial;
        }

        public void ApplyTransitionMaterial()
        {
            _decorationContainer.ApplyMaterialToAllDecorations(_transitionMaterial);
        
            _spawnerMesh.sharedMaterial = _terrainAnimator.TransitionMaterial;;
        }  

        private void ApplyBaseMaterial()
        {
            _decorationContainer.ApplyMaterialToAllDecorations(_baseMaterial);

            _spawnerMesh.sharedMaterial = _baseSpawnerMaterial;
        }

        public void SetCenterToTransitionMaterial(Vector3 position)
        {
            _transitionMaterial.SetVector("Center", position);
        }

        public void SetRaduisToTransitionMaterial(float radius)
        {
            _transitionMaterial.SetFloat("Distance", radius);
        }
    }
}