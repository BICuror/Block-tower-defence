using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public class MeshGenerator: MonoBehaviour
    {
        #region FaceData
        protected readonly Vector3[] RightFace = new Vector3[]
        {
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f)
        };
        protected readonly int[] RightTris = new int[] { 0, 2, 1, 0, 3, 2 };
        
        protected readonly Vector3[] LeftFace = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f)
        };
        protected readonly int[] LeftTris = new int[] { 0, 1, 2, 0, 2, 3 };

        protected readonly Vector3[] UpFace = new Vector3[]
        {
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f)
        };
        protected readonly int[] UpTris = new int[] { 0, 1, 2, 0, 2, 3 };

        protected readonly Vector3[] ForwardFace = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f)
        };
        protected readonly int[] ForwardTris = new int[] { 0, 2, 1, 0, 3, 2 };

        protected readonly Vector3[] BackFace = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        };
        protected readonly int[] BackTris = new int[] { 0, 1, 2, 0, 2, 3 };
        #endregion
        
        #region FaceUVData
        protected readonly int[] XUVOrder = new int[] { 2, 3, 1, 0 };
        protected readonly int[] YUVOrder = new int[] { 0, 1, 3, 2 };
        protected readonly int[] ZUVOrder = new int[] { 3, 1, 0, 2 };
        #endregion
        
        protected struct FaceData
        {
            public FaceData(Vector3[] verticies, int[] indices, int[] newUVOrder)
            {
                Verticies = verticies;
                Indices = indices;
                UVOrder = newUVOrder;
            }

            public Vector3[] Verticies;
            public int[] Indices;
            public int[] UVOrder;
        }

        private Dictionary <Vector3Int, FaceData> _cubeFaces;

        private Vector3Int[] _allCheckDirections = new Vector3Int[5]
        {
            Vector3Int.right,
            Vector3Int.left,
            Vector3Int.up,
            Vector3Int.forward,
            Vector3Int.back
        };

        private Vector3Int[] _wallCheckDirections = new Vector3Int[4]
        {
            Vector3Int.right,
            Vector3Int.left,
            Vector3Int.forward,
            Vector3Int.back
        };

        protected BlockGrid _blockGrid;
        protected TextureManager _textureManager;

        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _indices = new List<int>();
        protected List<Vector2> UVs = new List<Vector2>();

        public void SetupGenerator(BlockGrid blockGrid, TextureManager textureManager)
        {
            _blockGrid = blockGrid;
            _textureManager = textureManager;

            _cubeFaces = new Dictionary <Vector3Int, FaceData>();

            _cubeFaces.Add(Vector3Int.right, new FaceData(RightFace, RightTris, XUVOrder));
            _cubeFaces.Add(Vector3Int.left, new FaceData(LeftFace, LeftTris, XUVOrder));
            _cubeFaces.Add(Vector3Int.up, new FaceData(UpFace, UpTris, YUVOrder));
            _cubeFaces.Add(Vector3Int.forward, new FaceData(ForwardFace, ForwardTris, ZUVOrder));
            _cubeFaces.Add(Vector3Int.back, new FaceData(BackFace, BackTris, ZUVOrder));
        }

        public Mesh GetMesh()
        {
            for (int x = 0; x < _blockGrid.GetSize(); x++)
            {
                for (int z = 0; z < _blockGrid.GetSize(); z++)
                {
                    CheckAroundPositionToCreateWall(new Vector3Int(x, 0, z));
                    
                    for (int y = 0; y < _blockGrid.GetHeight(); y++)
                    {
                        CheckAroundPosition(new Vector3Int(x, y, z));
                    }
                }
            }

            Mesh mesh = new Mesh();

            mesh.SetVertices(_vertices);
            mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, UVs);

            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

            _vertices = new List<Vector3>();
            _indices = new List<int>();
            UVs = new List<Vector2>();

            return mesh;
        }

        private void CheckAroundPositionToCreateWall(Vector3Int position)
        {
            if (ShouldCheckThisBlockType(_blockGrid.GetBlockType(position)) == false) return; 

            for (int i = 0; i < _wallCheckDirections.Length; i++)
            {
                Vector3Int blockPositionToCheck = position + _wallCheckDirections[i];

                if (_blockGrid.IsInBounds(blockPositionToCheck) == true)
                {
                    if (ShouldCheckBlock(blockPositionToCheck) && _blockGrid.GridSpaceIsEmpty(blockPositionToCheck) && _blockGrid.GridSpaceIsEmpty(position) == false)
                    {   
                        SetWallFace(position, i);
                    }
                }
                else if (_blockGrid.GridSpaceIsEmpty(position) == false)
                {    
                    SetWallFace(position, i);   
                }
            }
        }

        protected virtual bool ShouldCheckBlock(Vector3Int positionToCheck) => true;

        private void SetWallFace(Vector3Int position, int index)
        {
            FaceData faceToApply = _cubeFaces[_wallCheckDirections[index]];

            ApplyTextureToFace(position, _wallCheckDirections[index], _blockGrid.GetBlockType(position), faceToApply);

            ApplyWallVerticies(faceToApply, position);
            
            ApplyIndecies(faceToApply);          
        }

        private void ApplyWallVerticies(FaceData faceToApply, Vector3Int position)
        {
            for (int verticyIndex = 0; verticyIndex < faceToApply.Verticies.Length; verticyIndex++)
            {
                Vector3 verticy = faceToApply.Verticies[verticyIndex];

                verticy.y = (verticy.y - 0.5f) * 20f - 0.5f;

                _vertices.Add(new Vector3(position.x, position.y, position.z) + verticy); 
            }
        }


        private void CheckAroundPosition(Vector3Int position)
        {
            if (ShouldCheckThisBlockType(_blockGrid.GetBlockType(position)) == false) return; 

            for (int i = 0; i < _allCheckDirections.Length; i++)
            {
                Vector3Int blockPositionToCheck = position + _allCheckDirections[i];

                if (_blockGrid.IsInBounds(blockPositionToCheck) == true)
                {
                    if (ShouldCheckBlock(blockPositionToCheck) && _blockGrid.GridSpaceIsEmpty(blockPositionToCheck) && _blockGrid.GridSpaceIsEmpty(position) == false)
                    {   
                        SetFace(position, i);
                    }
                }
                else if (_blockGrid.GridSpaceIsEmpty(position) == false)
                {    
                    SetFace(position, i);   
                }
            }
        }

        protected virtual bool ShouldCheckThisBlockType(BlockType type) => true;

        private void SetFace(Vector3Int position, int index)
        {
            FaceData faceToApply = _cubeFaces[_allCheckDirections[index]];

            ApplyTextureToFace(position, _allCheckDirections[index], _blockGrid.GetBlockType(position), faceToApply);

            ApplyVerticies(faceToApply, position);
            
            ApplyIndecies(faceToApply);          
        }

        protected virtual void ApplyTextureToFace(Vector3Int position, Vector3Int checkDirection, BlockType blockType, FaceData faceToApply) {}

        private void ApplyVerticies(FaceData faceToApply, Vector3Int position)
        {
            for (int verticyIndex = 0; verticyIndex < faceToApply.Verticies.Length; verticyIndex++)
            {
                _vertices.Add(new Vector3(position.x, position.y, position.z) + faceToApply.Verticies[verticyIndex]); 
            }
        }

        private void ApplyIndecies(FaceData faceToApply)
        {
            for (int indecyIndex = 0; indecyIndex < faceToApply.Indices.Length; indecyIndex++)
            {
                _indices.Add(_vertices.Count - 4 + faceToApply.Indices[indecyIndex]); 
            }
        }
    }
}