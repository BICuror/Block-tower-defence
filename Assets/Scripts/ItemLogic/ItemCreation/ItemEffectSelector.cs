using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using Random = UnityEngine.Random;

public sealed class ItemEffectSelector : MonoBehaviour
{
    [SerializeField] private ItemFactory _itemFactory;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    [Inject] private ItemsContainer _itemsContainer;

    #region ModifiersSelection

    public List<List<ToggleGlobalEffectData>> GetItemEffects(int totalStrength, int minStrength, int itemAmount)
    {
        List<List<ToggleGlobalEffectData>> resultEffects = new();
        
        int iterations = 50;
        
        do
        {
            iterations--;

            resultEffects.Clear();
            
            List<int> itemStrengths = GetRandomItemStrengths(totalStrength, minStrength, itemAmount);

            for (int i = 0; i < itemAmount; i++)
            {
                if (TryGetItemEffectDatas(itemStrengths[i], true, out List<ToggleGlobalEffectData> effects))
                {
                    resultEffects.Add(effects);
                }
                else break;
                
                if (resultEffects.Count != i + 1) break;
            }
            
            if (iterations <= 0) break;
        } 
        while (itemAmount != resultEffects.Count);
        
        if (itemAmount != resultEffects.Count) Debug.LogError("Incorrect total strength");
        if (iterations == 0) Debug.LogError("Iterations over 50");
        
        return resultEffects;
    }

    private List<int> GetRandomItemStrengths(int totalStrength, int minStrength, int itemAmount)
    {
        List<int> result = new();
        
        int maxItemStrength = totalStrength - minStrength * (itemAmount - 1);
        
        List<int> nonEmptyStrengths = PopulateNonEmptyStrengthList(totalStrength).FindAll(strength => strength >= minStrength && strength <= maxItemStrength);

        int leftStrength = totalStrength;
        
        for (int i = 0; i < itemAmount; i++)
        {
            int currentItemStrength = nonEmptyStrengths[Random.Range(0, nonEmptyStrengths.Count)];
            
            if (itemAmount - 1 == i) currentItemStrength = leftStrength;
            
            result.Add(currentItemStrength);
            
            leftStrength -= currentItemStrength;
            
            nonEmptyStrengths.Remove(currentItemStrength);
        }
        
        return result;
    }
    
    public bool TryGetItemEffectDatas(int strength, bool forceMeetStrength, out List<ToggleGlobalEffectData> result)
    {
        result = new();
        
        List<ToggleGlobalEffectData> effectDatas = new(_islandDataContainer.Data.ItemToggleEffectContainer.EffectDatas);
        
        Debug.Log($"Trying to find item effect data for {strength}");
        List<int> nonEmptyStrengths = PopulateNonEmptyStrengthList(strength).Intersect(GetValidEffectStrengthList()).ToList();
        
        int leftStrength = strength;

        while (nonEmptyStrengths.Count > 0)
        {
            int currentStrength = nonEmptyStrengths[Random.Range(0, nonEmptyStrengths.Count)];

            if (Random.Range(0, 100) < 50) currentStrength = nonEmptyStrengths.Max();

            if (forceMeetStrength && currentStrength > leftStrength)
            {
                nonEmptyStrengths.Remove(currentStrength);
                continue;
            }

            if (TryGetRandomEffectDataOfStrength(currentStrength, effectDatas, result.ConvertAll(data => (GlobalEffectData)data), out ToggleGlobalEffectData effectData))
            {
                effectDatas.Remove(effectData);
                result.Add(effectData);

                leftStrength -= currentStrength;
            }
            else nonEmptyStrengths.Remove(currentStrength);
            
            if (leftStrength <= 0) return leftStrength == 0;
        }

        return leftStrength == 0;
    }

    private List<int> GetValidEffectStrengthList()
    {
        List<int> result = new();
        
        _islandDataContainer.Data.ItemToggleEffectContainer.EffectDatas.ForEach(effectData =>
        {
            if (!result.Contains(effectData.Quality)) result.Add(effectData.Quality);
        });
        
        return result;
    }
    
    private List<int> PopulateNonEmptyStrengthList(int strength)
    {
        List<int> result = new();

        while (strength > 0)
        {
            result.Add(strength);
            strength--;
        }

        return result;
    }
    
    private bool TryGetRandomEffectDataOfStrength(int strength, List<ToggleGlobalEffectData> datas, List<GlobalEffectData> exsistingEffects, out ToggleGlobalEffectData data)
    {
        List<ToggleGlobalEffectData> selectedDatas = datas.FindAll(selectedData => selectedData.Quality == strength);
        data = null;
        
        for (int i = 0; i < selectedDatas.Count; i++)
        {
            int randomIndex = Random.Range(0, selectedDatas.Count);
            ToggleGlobalEffectData selectedData = selectedDatas[randomIndex];
            selectedDatas.RemoveAt(randomIndex);
         
            if (selectedData.IsUnique && TryToFindExistingEffectData(selectedData)) continue;

            if (!CheckIfEffectTagRequirementsAreMet(selectedData, exsistingEffects)) continue;
            
            if (selectedData.HasAppearanceCondition)
            {
                if (_globalEffectFactory.CanAppear(selectedData))
                {
                    data = selectedData; 
                    return true;
                }
            }
            else
            {
                data = selectedData; 
                return true;
            }
        }
        
        Debug.LogError($"Couldn't find any effect for {strength}");

        return false;
    }

    private bool TryToFindExistingEffectData(ToggleGlobalEffectData effectData)
    {
        return _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().Exists(item => item.ToggleEffectDatas.Exists(data => data == effectData));
    }

    private bool CheckIfEffectTagRequirementsAreMet<T>(T effectData, List<GlobalEffectData> globalEffectDatas) where T : GlobalEffectData
    {
        if (!effectData.HasRequiredTags && !effectData.HasBlockTags) return true;
        
        List<EntityModifcatorTag> buildingsTags = _globalBuildingContainer.GetBuildingTags();
        
        _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().ForEach(item =>
        {
            globalEffectDatas.AddRange(item.ToggleEffectDatas);
        });
        
        Debug.Log($"Checking requirements for {effectData.name}");
        
        if (effectData.HasRequiredTags) 
        {
            if (effectData.RequiredBuildingTags.Count > 0 && !EntityTagRequirementsChecker.RequirementsAreMet(buildingsTags, effectData.RequiredBuildingTags)) return false;
            if (effectData.RequiredGlobalEffectsTags.Count > 0 && !GlobalEffectTagRequirementsChecker.RequirementsAreMet(globalEffectDatas, effectData.RequiredGlobalEffectsTags)) return false;
        }
        if (effectData.HasBlockTags)
        {
            if (effectData.BlockBuildingsTags.Count > 0 && EntityTagRequirementsChecker.RequirementsAreMet(buildingsTags, effectData.BlockBuildingsTags)) return false;
            if (effectData.BlockGlobalEffectTags.Count > 0 && GlobalEffectTagRequirementsChecker.RequirementsAreMet(globalEffectDatas, effectData.BlockGlobalEffectTags)) return false;
        }
        
        Debug.Log($"Suckseful Checking requirements for {effectData.name}");
        
        return true;
    }
    
    #endregion
}