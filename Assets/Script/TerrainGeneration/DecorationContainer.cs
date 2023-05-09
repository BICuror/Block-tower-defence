using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DecorationContainer : MonoBehaviour
{
    private GameObject[,][] _decorations;

    private int _areaSize;

    public void CreateNewContainer(int size)
    {
        _areaSize = size;

        _decorations = new GameObject[_areaSize, _areaSize][];
    } 

    public void AddDecorations(int x, int z, GameObject[] decorationsToAdd)
    {
        _decorations[x, z] = decorationsToAdd;
    }

    public void ActivateAllDecorations()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
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
                _decorations[x, z][i].SetActive(false);
            }
        }
    }
}
