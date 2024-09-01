using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "CubeTextures", menuName = "Generation/CubeTextures")]

    public sealed class CubeTextures : ScriptableObject
    {       
        [SerializeField] private Sprite[] _xTexture;
        [SerializeField] private Sprite[] _yTexture;
        [SerializeField] private Sprite[] _zTexture;
            
        public Vector2[] GetUVsAtDirection(Vector3Int direction)
        {
            if (direction == Vector3.right || direction == Vector3.left) return _xTexture[Random.Range(0, _xTexture.Length)].uv;
            if (direction == Vector3.down || direction == Vector3.up) return _yTexture[Random.Range(0, _yTexture.Length)].uv;
            if (direction == Vector3.back || direction == Vector3.forward) return _zTexture[Random.Range(0, _zTexture.Length)].uv;

            return null;
        }
    }
}