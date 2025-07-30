using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RoadPartData))]

public sealed class GridDataEditor : Editor
{
    private RoadPartData _roadPartData;
    private GUILayoutOption[] _buttonLayout = new GUILayoutOption[2]
    {
        GUILayout.MaxWidth(30),
        GUILayout.MaxHeight(30)
    };
    
    private SerializedProperty _tileTypeGrid;
    private SerializedProperty _size;
    private SerializedProperty _gridDataType;
    private SerializedProperty _gridMirrorMode;
    

    private void OnEnable() 
    {
        _tileTypeGrid = serializedObject.FindProperty("TileGrid");
        _size = serializedObject.FindProperty("Size");
        _gridDataType = serializedObject.FindProperty("GridDataType");
        _gridMirrorMode = serializedObject.FindProperty("GridMirrorMode");
    }   
 
    public override void OnInspectorGUI()
    {
        if (_tileTypeGrid == null) return;
        serializedObject.Update();
 
        _roadPartData = (RoadPartData)target;
        _roadPartData.FillTileGridIfEmpty();

        DrawWitdhHeightSliders();

        DrawButtons();
 
        DrawGrid();
 
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWitdhHeightSliders()
    {
        int initialSize = _roadPartData.Size;

        int size = EditorGUILayout.IntSlider("Size", _roadPartData.Size, 1, 25); 
        _size.intValue = size;

        if (initialSize != size) _roadPartData.TransferTilesToNewGrid();
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("ResetGrid")) 
        {
            _roadPartData.ReinstantiateGrid();
        }
    }
 
    //TO DO replace GUILayout calls
    private void DrawGrid()
    {
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        
        for (int y = 0; y < _roadPartData.Size; y++)
        {    
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SerializedProperty tileTypeColumn = _tileTypeGrid.GetArrayElementAtIndex(y).FindPropertyRelative("TileTypesRow");
            
            for (int x = 0; x < _roadPartData.Size; x++)
            {
                SerializedProperty tile = tileTypeColumn.GetArrayElementAtIndex(x);
                RoadPartTileState tileType = (RoadPartTileState)tile.intValue;
                
                GUI.color = GetTileColor(tileType);

                if (GUILayout.Button("", _buttonLayout))
                {
                    RoadPartTileState newTileType = SwitchTileState(tileType);

                    tile.intValue = (int)newTileType;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
    }
 
    private RoadPartTileState SwitchTileState(RoadPartTileState initialType)
    {
        int typeIndex = (int)initialType;

        typeIndex++;
        
        if (Enum.GetValues(typeof(RoadPartTileState)).Length <= typeIndex) typeIndex = 0;
    
        return (RoadPartTileState)typeIndex;
    }

    private Color32 GetTileColor(RoadPartTileState tileType)
    {
        switch (tileType)
        {
            case RoadPartTileState.Empty: return new Color32(70, 70, 70, 50);
            case RoadPartTileState.EmptySpaceRequired: return new Color32(110, 25, 25, 255);
            case RoadPartTileState.Solid: return new Color32(220, 220, 220, 255);
            case RoadPartTileState.Entrance: return new Color32(75, 220, 75, 255);
            case RoadPartTileState.Exit: return new Color32(220, 75, 75, 255);
            default: throw new NotImplementedException();
        }
    }
}