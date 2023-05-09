using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationGenerator : MonoBehaviour
{
    [SerializeField] private DecorationContainer _decorationContainer;

    public void GenerateDecorations(BlockGrid blockGrid, Vector2Int offset)
    {
        int areaSize = blockGrid.GetSize();

        _decorationContainer.CreateNewContainer(areaSize);

        IslandData.DecorationModule decorationModule;

        for (int x = 0; x < areaSize; x++)
        {
            for (int z = 0; z < areaSize; z++)
            {
                decorationModule = GetDecorationModule(x, z);

                if (decorationModule.DecorationAppearRate > Random.Range(0f, 1f) && blockGrid.GetMaxHeight(x, z) > 0)
                {         
                    CreateDecorations(GetRandomDecoration(decorationModule.Decorations), new Vector3Int(x + offset.x, blockGrid.GetMaxHeight(x, z), z + offset.y));
                }
            }
        }
    }

    protected virtual IslandData.DecorationModule GetDecorationModule(int x, int z) => new IslandData.DecorationModule();

    private void CreateDecorations(IslandData.Decoration decoration, Vector3Int position)
    {
        int amountOfDecorations = Random.Range(0, decoration.Amount); 

        GameObject[] decorations = new GameObject[amountOfDecorations];

        for (int i = 0; i < amountOfDecorations; i++)
        {
            Vector3 spawnPosition = position + new Vector3(Random.Range(-decoration.PlacmentOffset, decoration.PlacmentOffset), 0.5f, Random.Range(-decoration.PlacmentOffset, decoration.PlacmentOffset));

            float randomXRot = Random.Range(-decoration.MaxXRotation, decoration.MaxXRotation);    
            float randomYRot = Random.Range(-decoration.MaxYRotation, decoration.MaxYRotation); 
            float randomZRot = Random.Range(-decoration.MaxZRotation, decoration.MaxZRotation); 

            decorations[i] = Instantiate(decoration.Prefabs[Random.Range(0, decoration.Prefabs.Length)], spawnPosition, Quaternion.Euler(randomXRot, randomYRot, randomZRot), transform);                    

            float randomScale = Random.Range(decoration.MinScale, decoration.MaxScale);
            decorations[i].transform.localScale = new Vector3(randomScale, randomScale, randomScale); 
        }

        _decorationContainer.AddDecorations(position.x, position.z, decorations);
    }

    private IslandData.Decoration GetRandomDecoration(IslandData.Decoration[] decorations)
    {
        float random = Random.Range(0f, 1f);

        for (int i = 0; i < decorations.Length; i++)
        {
            if (random < decorations[i].AppearRate) return decorations[i];
        }

        Debug.Log("Decoration not found");
        return new IslandData.Decoration();
    }
}
