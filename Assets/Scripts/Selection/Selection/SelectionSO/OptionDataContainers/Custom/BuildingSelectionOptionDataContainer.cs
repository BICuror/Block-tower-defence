using System.Collections.Generic;
using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "BuildingSelectionOptionDataContainer", menuName = "Selection/OptionDataContainers/BuildingSelectionOptionDataContainer")]

public sealed class BuildingSelectionOptionDataContainer : ScriptableObject
{
    [SerializeField] private List<BuildingEntity> _optionDatas;
    
    public List<BuildingEntity> GetRandomPrefabs(int amount)
    {
        List<BuildingEntity> datas = new(_optionDatas);
        
        List<BuildingEntity> result = new();

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, datas.Count);

            result.Add(datas[randomIndex]);
            datas.RemoveAt(randomIndex);
        }

        return result;
    }
}