using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public sealed class NodeGenerator : MonoBehaviour
{
    private List<int> _xNodes;
    private List<int> _yNodes;

    public void SetupNodes()
    {
        IslandData islandData = IslandDataContainer.GetData();

        List<int> allXValues = GetAllPossibleValusesInRange(islandData.IslandSize);
        List<int> allYValues = GetAllPossibleValusesInRange(islandData.IslandSize);
        
        _xNodes = GetNodes(allXValues, islandData.AmountOfRoadNodesBetweenCenterAndEdge);
        _yNodes = GetNodes(allYValues, islandData.AmountOfRoadNodesBetweenCenterAndEdge);
    }

    public Vector2Int[,] GetAllNodes()
    {
        IslandData islandData = IslandDataContainer.GetData();

        Vector2Int[,] nodes = new Vector2Int[islandData.IslandSize, islandData.IslandSize];
        
        for (int x = 0; x < _xNodes.Count; x++)
        {
            for (int y = 0; y < _yNodes.Count; y++)
            {
                nodes[x, y] = new Vector2Int(_xNodes[x], _yNodes[y]);
            }
        }
    
        return nodes;
    }

    public Vector2Int GetEnemySpawnerNodes(List<Vector2Int> exclusiveList)
    {
        List<Vector2Int> possibleNodes = new List<Vector2Int>();

        for (int x = 0; x < _xNodes.Count; x++)
        {
            for (int y = 0; y < _yNodes.Count; y++)
            {
                if ((x == 1 || x == _xNodes.Count - 2) && (y == 1 || y == _yNodes.Count - 2)) 
                {
                    possibleNodes.Add(new Vector2Int(_xNodes[x], _yNodes[y]));     
                }
            }
        }
        
        for (int x = 0; x < exclusiveList.Count; x++)
        {
            for (int y = 0; y < possibleNodes.Count; y++)
            {
                if (exclusiveList[x] == possibleNodes[y])
                {
                    possibleNodes.RemoveAt(y);

                    break;
                }
            }
        }
        
        return possibleNodes[Random.Range(0, possibleNodes.Count)];
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

        AddNode(nodes, values, values[0]);
        AddNode(nodes, values, Mathf.RoundToInt(values.Count / 2) + 1);
        AddNode(nodes, values, values[values.Count - 1]);
        
        for (int i = 0; i < amountOfNodesBetweenCenterAndEdge; i++)
        {
            List<int> valuesToRemove = values.FindAll(x => x > Mathf.RoundToInt(valuesCount / 2) + 1 && x < valuesCount);

            AddNode(nodes, values, valuesToRemove[Random.Range(0, valuesToRemove.Count)]);
        }  
        
        for (int i = 0; i < amountOfNodesBetweenCenterAndEdge; i++)
        {
            List<int> valuesToRemove = values.FindAll(x => x > 0 && x < Mathf.RoundToInt(valuesCount / 2) + 1);

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
