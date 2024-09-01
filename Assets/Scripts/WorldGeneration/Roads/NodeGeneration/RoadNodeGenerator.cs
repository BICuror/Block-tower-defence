using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class RoadNodeGenerator
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        private List<int> _xNodes;
        public IReadOnlyList<int> XNodes => _xNodes;
        
        private List<int> _zNodes;
        public IReadOnlyList<int> ZNodes => _zNodes;

        private Vector2Int[,] _nodes;

        public void SetupNodes()
        {
            List<int> allXValues = GetAllPossibleValusesInRange(_islandData.IslandSize);
            List<int> allZValues = GetAllPossibleValusesInRange(_islandData.IslandSize);
            
            _xNodes = GetNodes(allXValues, _islandData.AmountOfRoadNodesBetweenCenterAndEdge);
            _zNodes = GetNodes(allZValues, _islandData.AmountOfRoadNodesBetweenCenterAndEdge);

            GenerateNodes();
        }

        public Vector2Int[,] GetAllNodes() => _nodes;
        public Vector2Int GetNodePosition(Vector2Int index) => _nodes[index.x, index.y];

        private void GenerateNodes()
        {
            _nodes = new Vector2Int[_islandData.IslandSize, _islandData.IslandSize];
            
            for (int x = 0; x < _xNodes.Count; x++)
            {
                for (int z = 0; z < _zNodes.Count; z++)
                {
                    _nodes[x, z] = new Vector2Int(_xNodes[x], _zNodes[z]);
                }
            }
        }
        
        private List<int> GetAllPossibleValusesInRange(int maxValue)
        {
            List<int> values = new List<int>();

            for(int i = 0; i < maxValue; i++)
            {
                values.Add(i);
            }

            return values;
        }

        private List<int> GetNodes(List<int> values, int amountOfNodesBetweenCenterAndEdge)
        {
            List<int> nodes = new List<int>();

            int valuesCount = values.Count;

            int center = Mathf.RoundToInt(valuesCount / 2) + 1;

            AddNode(nodes, values, values[0]);
            AddNode(nodes, values, Mathf.RoundToInt(values.Count / 2) + 1);
            AddNode(nodes, values, values[values.Count - 1]);
            
            for (int i = 0; i < amountOfNodesBetweenCenterAndEdge; i++)
            {
                List<int> valuesToRemove = values.FindAll(j => j > center && j < valuesCount);

                AddNode(nodes, values, valuesToRemove[Random.Range(0, valuesToRemove.Count)]);
            }  
            
            for (int i = 0; i < amountOfNodesBetweenCenterAndEdge; i++)
            {
                List<int> valuesToRemove = values.FindAll(j => j > 0 && j < center);

                AddNode(nodes, values, valuesToRemove[Random.Range(0, valuesToRemove.Count)]);
            }
            
            nodes.Sort();

            return nodes;
        }

        private void AddNode(List<int> nodes, List<int> values, int valueToRemove)
        {
            nodes.Add(valueToRemove);

            values.Remove(valueToRemove + 1);
            values.Remove(valueToRemove - 1);
            values.Remove(valueToRemove);
        }
    }
}