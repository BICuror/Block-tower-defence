using UnityEngine;

namespace WorldGeneration
{
    public class TerrainSetter : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        [SerializeField] private MeshRenderer _meshRenderer;

        public void SetMesh(Mesh meshToSet)
        {
            _meshFilter.mesh = meshToSet;

            if (_meshCollider != null) _meshCollider.sharedMesh = meshToSet;
        }

        public void SetMaterial(Material materialToSet)
        {
            _meshRenderer.material = materialToSet;
        }
    }
}
