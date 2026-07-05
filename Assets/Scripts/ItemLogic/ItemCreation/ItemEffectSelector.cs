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

    public List<List<GlobalEffectData>> GetItemEffects(int totalStrength, int minStrength, int itemAmount)
    {
        List<List<GlobalEffectData>> resultEffects = new();
        
        int iterations = 50;
        
        do
        {
            List<GlobalEffectData> allSelectedEffects = new();
            
            iterations--;

            resultEffects.Clear();
            
            List<int> itemStrengths = GetRandomItemStrengths(totalStrength, minStrength, itemAmount);

            for (int i = 0; i < itemAmount; i++)
            {
                if (TryGetItemEffectDatas(itemStrengths[i], allSelectedEffects, true, out List<GlobalEffectData> effects))
                {
                    allSelectedEffects.AddRange(effects);
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

        int itemsLeftToGenerate = itemAmount;
        int leftStrength = totalStrength;
        
        for (int i = 0; i < itemAmount; i++)
        {
            int minStrengthRequiredForOtherItems = minStrength * (itemsLeftToGenerate - 1);
            int maxItemStrength = leftStrength - minStrengthRequiredForOtherItems;
            
            List<int> nonEmptyStrengths = PopulateNonEmptyStrengthList(maxItemStrength).FindAll(strength => strength >= minStrength && strength <= maxItemStrength);
            
            int currentItemStrength = nonEmptyStrengths[Random.Range(0, nonEmptyStrengths.Count)];
            
            if (itemsLeftToGenerate == 1) currentItemStrength = leftStrength;
            
            result.Add(currentItemStrength);
            
            leftStrength -= currentItemStrength;

            itemsLeftToGenerate -= 1;
        }
        
        if (result.Contains(0)) Debug.LogError("FOUND 0");
        
        return result;
    }
    
    public bool TryGetItemEffectDatas(int strength, List<GlobalEffectData> additionalExistingEffects, bool forceMeetStrength, out List<GlobalEffectData> result)
    {
        List<GlobalEffectData> allExistingEffects = new(additionalExistingEffects);
        _itemsContainer.ContainedItems.ForEach(item => item.EffectDatas.ForEach(data => allExistingEffects.Add(data)));
        
        result = new();
        
        List<GlobalEffectData> allUnselectedEffectDatas = new(_islandDataContainer.Data.ItemToggleEffectContainer.EffectDatas);
        
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
            
            if (TryGetRandomEffectDataOfStrength(currentStrength, allUnselectedEffectDatas, allExistingEffects, out GlobalEffectData effectData))
            {
                allUnselectedEffectDatas.Remove(effectData);
                allExistingEffects.Add(effectData);
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
    
    private bool TryGetRandomEffectDataOfStrength(int strength, List<GlobalEffectData> datas, List<GlobalEffectData> existingEffects, out GlobalEffectData data)
    {
        List<GlobalEffectData> selectedDatas = datas.FindAll(selectedData => selectedData.Quality == strength);
        data = null;
        
        for (int i = 0; i < selectedDatas.Count; i++)
        {
            int randomIndex = Random.Range(0, selectedDatas.Count);
            GlobalEffectData selectedData = selectedDatas[randomIndex];
            selectedDatas.RemoveAt(randomIndex);
         
            if (selectedData.HasStacks && existingEffects.Count(data => data == selectedData) >= selectedData.MaxStacks) continue;

            if (!CheckIfEffectTagRequirementsAreMet(selectedData, existingEffects)) continue;
            
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

    private bool CheckIfEffectTagRequirementsAreMet<T>(T effectData, List<GlobalEffectData> globalEffectDatas) where T : GlobalEffectData
    {
        if (!effectData.HasRequiredTags && !effectData.HasBlockTags) return true;
        
        List<EntityModifcatorTag> buildingsTags = _globalBuildingContainer.GetBuildingTags();
        
        _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().ForEach(item =>
        {
            globalEffectDatas.AddRange(item.EffectDatas);
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
        
        Debug.Log($"Successful Checking requirements for {effectData.name}");
        
        return true;
    }
    
    #endregion
}