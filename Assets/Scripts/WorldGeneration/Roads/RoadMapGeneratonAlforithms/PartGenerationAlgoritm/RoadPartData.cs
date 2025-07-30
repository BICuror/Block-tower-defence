using UnityEngine;
using System;

[CreateAssetMenu(fileName = "RoadPartData", menuName = "Data/RoadPartData")]

public sealed class RoadPartData : ScriptableObject
{
    public int Size = 5;
    public Row[] TileGrid;
    
    private void OnValidate()
    {
        if (TileGrid.Length != Size || TileGrid[0].GetLength() != Size)
        {
            TransferTilesToNewGrid();
        }
    }

    public RoadPartTileState[,] GetTileGrid()
    {
        RoadPartTileState[,] tileGrid = new RoadPartTileState[Size, Size];

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                tileGrid[x, Size - 1 - y] = TileGrid[y].Get(x);
            }
        }

        return tileGrid;
    }
    
    public void FillTileGridIfEmpty()
    {
        if (TileGrid == null) ReinstantiateGrid();
    }
    
    public void ReinstantiateGrid()
    {
        TileGrid = new Row[Size];

        for (int x = 0; x < TileGrid.Length; x++)
        {
            TileGrid[x] = new Row(Size);
        }
    }

    public void TransferTilesToNewGrid()
    {
        Row[] copyTileGrid = TileGrid;

        ReinstantiateGrid();

        int copyWitdh = Mathf.Min(Size, copyTileGrid[0].GetLength());
        int copyHeight = Mathf.Min(Size, copyTileGrid.Length);

        for (int y = 0; y < copyHeight; y++)
        {
            for (int x = 0; x < copyWitdh; x++)
            {
                TileGrid[y].Set(x, copyTileGrid[y].Get(x));
            }
        }
    }
}

[Serializable] public struct Row
{
    public RoadPartTileState[] TileTypesRow;

    public Row(int Length)
    {
        TileTypesRow = new RoadPartTileState[Length];
    }

    public void Set(int index, RoadPartTileState value) => TileTypesRow[index] = value;
    public RoadPartTileState Get(int index) => TileTypesRow[index];
    public int GetLength() => TileTypesRow.GetLength(0);
}

public enum RoadPartTileState
{
    Empty,
    EmptySpaceRequired,
    Solid,
    Entrance, 
    Exit  
}