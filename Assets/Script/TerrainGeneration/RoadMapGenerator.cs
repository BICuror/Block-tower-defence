using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;

public class RoadMapGenerator : MonoBehaviour
{
    private bool[,] _roadMap;

    private bool[,] _touchedNodesMap;

    public bool[,] GenerateRoads(List<Vector2Int> spawnerNodes, Vector2Int[,] roadNodes)
    {
        IslandData islandData = IslandDataContainer.GetData();

        _roadMap = new bool[islandData.IslandSize, islandData.IslandSize];

        _touchedNodesMap = new bool[islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 3, islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 3];

        for (int i = 0; i < spawnerNodes.Count; i++)
        {   
            Debug.Log(spawnerNodes[i].x.ToString() + " " + spawnerNodes[i].y.ToString());

            CreateSpawnerRoad(spawnerNodes, roadNodes, i);
        }

        return _roadMap;
    }

    private void CreateSpawnerRoad(List<Vector2Int> spawnerNodes, Vector2Int[,] roadNodes, int index)
    {
        IslandData islandData = IslandDataContainer.GetData();

        Vector2Int current = FindNodeIndex(spawnerNodes[index], roadNodes, islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 3);

        int middleIndex = (islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 2) / 2;

        _roadMap[roadNodes[current.x, current.y].x, roadNodes[current.x, current.y].y] = true;

        _touchedNodesMap[current.x, current.y] = true;

        int iterator = 0;
        while (current.x != middleIndex && current.y != middleIndex)
        {
            Vector2Int next = GetNextNodeIndex(middleIndex, current.x, current.y);

            MoveRoad(roadNodes[current.x, current.y], roadNodes[current.x + next.x, current.y + next.y]);

            current.x += next.x;
            current.y += next.y;

            _touchedNodesMap[current.x, current.y] = true;

            if (iterator > 20) 
            {
                MoveRoad(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]); 
                current.x = middleIndex;
                current.y = middleIndex;
            }     
            iterator++;
        }

        MoveRoad(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]);
    }

    private Vector2Int FindNodeIndex(Vector2Int position, Vector2Int[,] nodes, int amountOfNodes)
    {
        for (int x = 0; x < amountOfNodes; x++)
        {
            for(int y = 0; y < amountOfNodes; y++)
            {
                if (position.x == nodes[x, y].x && position.y == nodes[x, y].y) return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(0,0);
    }

    private Vector2Int GetNextNodeIndex(int middleIndex, int xIndex, int yIndex)
    {
        int xDifference = NormalizeNumber(middleIndex - xIndex);
        int yDifference = NormalizeNumber(middleIndex - yIndex);

        Vector2Int nextNodeIndex = new Vector2Int(0,0);

        if (xIndex > 0 && xIndex < middleIndex * 2 - 1 && yIndex > 0 && yIndex < middleIndex * 2 - 1)
        {
            if (xDifference == 0) 
            {
                if (Random.Range(0, 100) > 50 && NodeIsUnouched(xIndex - 1, yIndex)) xDifference = -1;
                else if (NodeIsUnouched(xIndex + 1, yIndex)) xDifference = 1;
            }
            else if (yDifference == 0)
            {
                if (Random.Range(0, 100) > 50 && NodeIsUnouched(xIndex, yIndex - 1)) yDifference = -1;
                else if (NodeIsUnouched(xIndex, yIndex - 1)) yDifference = 1;
            }
            else if (Random.Range(0, 100) > 50 && NodeIsUnouched(xIndex, yIndex - yDifference)) yDifference *= -1;
            else if (NodeIsUnouched(xIndex - xDifference, yIndex)) xDifference *= -1;

            return new Vector2Int(xDifference, yDifference);
        }
    
        if (xDifference != 0 && yDifference == 0 && NodeIsUnouched(xIndex + xDifference, yIndex)) nextNodeIndex = new Vector2Int(xDifference, 0);
        else if (yDifference != 0 && xDifference == 0 && NodeIsUnouched(xIndex, yIndex + yDifference)) nextNodeIndex = new Vector2Int(0, yDifference);
        else 
        {
            if (Random.Range(0, 100) > 50 && NodeIsUnouched(xIndex + xDifference, yIndex + yDifference))
            {
                nextNodeIndex = new Vector2Int(xDifference, yDifference);
            }
            else
            {
                if (Random.Range(0, 100) > 50 && NodeIsUnouched(xIndex, yIndex + yDifference)) nextNodeIndex = new Vector2Int(0, yDifference);
                else if (NodeIsUnouched(xIndex + xDifference, yIndex)) nextNodeIndex = new Vector2Int(xDifference, 0);
            }
        } 
        
        return nextNodeIndex;
    }

    private bool NodeIsUnouched(int x, int y) => _touchedNodesMap[x, y] == false;

    private void MoveRoad(Vector2Int currentPos, Vector2Int neededPos)
    {
        while(currentPos != neededPos)
        {
            Vector2Int moveDir = GetMoveDirection(currentPos, neededPos);

            currentPos = currentPos + moveDir;

            _roadMap[currentPos.x, currentPos.y] = true;
        }
    }

    private Vector2Int GetMoveDirection(Vector2Int currentPos, Vector2Int neededPos)
    {
        Vector2Int dirDifference = neededPos - currentPos;

        int xDifference = NormalizeNumber(dirDifference.x);
        int yDifference = NormalizeNumber(dirDifference.y);

        if (xDifference == 0) return new Vector2Int(0, yDifference);
        if (yDifference == 0) return new Vector2Int(xDifference, 0);
        
        if (Random.Range(0, 100) > 50) return new Vector2Int(0, yDifference);
        else return new Vector2Int(xDifference, 0);
    }

    private int NormalizeNumber(int num)
    {
        if (num == 0) return 0;
        return num / Mathf.Abs(num);
    }
}
