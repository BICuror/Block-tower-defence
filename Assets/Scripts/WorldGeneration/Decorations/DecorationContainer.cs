using UnityEngine;

namespace WorldGeneration
{
    public class DecorationContainer : MonoBehaviour
    {
        private MeshRenderer[,][] _decorations;

        private int _areaSize;

        public void CreateNewContainer(int size)
        {
            DestroyAllDecorations();

            _areaSize = size;

            _decorations = new MeshRenderer[_areaSize, _areaSize][];
        } 

        public void AddDecorations(int x, int z, MeshRenderer[] decorationsToAdd)
        {
            _decorations[x, z] = decorationsToAdd;
        }

        public void ActivateAllDecorations()
        {
            for (int x = 0; x < _areaSize; x++)
            {
                for (int y = 0; y < _areaSize; y++)
                {
                    for (int i = 0; i < _decorations[x, y]?.Length; i++)
                    {
                        _decorations[x, y][i].gameObject?.SetActive(true);
                    }
                }
            }
        }

        public void SetActiveDecorationsIfInBound(int x, int z, bool state)
        {
            if (x >= 0 && z >= 0 && x < _areaSize && z < _areaSize) SetActiveDecorations(x, z, state);
        }

        public void SetActiveDecorations(int x, int z, bool state)
        {
            if (_decorations[x, z] != null)
            {
                for (int i = 0; i < _decorations[x, z].Length; i++)
                {
                    _decorations[x, z][i].gameObject.SetActive(false);
                }
            }
        }

        public void DestroyAllDecorations()
        {
            for (int x = 0; x < _areaSize; x++)
            {
                for (int z = 0; z < _areaSize; z++)
                {
                    if (_decorations[x, z] != null)
                    {
                        for (int i = 0; i < _decorations[x, z].Length; i++)
                        {
                            Destroy(_decorations[x, z][i].gameObject);
                        }
                    }
                }
            }
        }

        public void ApplyMaterialToAllDecorations(Material materialToApply)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            for (int x = 0; x < _areaSize; x++)
            {
                for (int y = 0; y < _areaSize; y++)
                {
                    if (_decorations[x, y] != null)
                    {
                        for (int i = 0; i < _decorations[x, y].Length; i++)
                        {
                            _decorations[x, y][i].sharedMaterial = materialToApply;
                        
                            _decorations[x, y][i].SetPropertyBlock(block);
                        }
                    }
                }
            }
        }
    }
}
