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
    private ItemModifiersSelectionContainer _itemModifiersSelectionContainer;

    private void Awake()
    {
        _itemModifiersSelectionContainer = _islandDataContainer.Data.ItemModifiersSelectionContainer;
    }

    #region ModifiersSelection
    
    public List<ToggleGlobalEffectData> GetRandomToggleEffectDatas(int strength)
    {
        List<ToggleGlobalEffectData> result = new();

        List<ToggleGlobalEffectData> toggleEffectDatas = _itemModifiersSelectionContainer.ItemToggleEffectContainer.EffectDatas;

        //List<ToggleGlobalEffectData> positiveEffects = toggleEffectDatas.FindAll(effectData => effectData.EffectType == EffectType.Positive);
        List<ToggleGlobalEffectData> negativeEffects = toggleEffectDatas.FindAll(effectData => effectData.EffectType == EffectType.Negative); 

        //int positiveStrength = quality + strength;
        int negativeStrength = strength;

        //result.AddRange(GetItemEffectDatas(positiveStrength, positiveEffects));
        result.AddRange(GetItemEffectDatas(negativeStrength, negativeEffects));

        return result;   
    }
    
    private List<ToggleGlobalEffectData> GetItemEffectDatas(int strength, List<ToggleGlobalEffectData> itemEffectDats)
    {
        if (strength < 1) strength = 1;

        List<ToggleGlobalEffectData> effectDatas = new List<ToggleGlobalEffectData>(itemEffectDats);
        
        List<ToggleGlobalEffectData> result = new();
        Debug.Log($"Trying to find item effect data for {strength}");
        List<int> nonEmptyQualities = PopulateNonEmptyStrengthList(strength);
        int leftStrength = strength;
        
        while (leftStrength > 0 && nonEmptyQualities.Count > 0)
        {
            int currentStrength = nonEmptyQualities[Random.Range(0, nonEmptyQualities.Count)];

            if (TryGetRandomEffectData(currentStrength, new List<ToggleGlobalEffectData>(effectDatas), out ToggleGlobalEffectData effectData))
            {
                effectDatas.Remove(effectData);
                result.Add(effectData);

                leftStrength -= currentStrength;
            }
            else
            {
                nonEmptyQualities.Remove(currentStrength);
            }
        }

        return result;
    }
    
    private List<int> PopulateNonEmptyStrengthList(int strength)
    {
        List<int> result = new();
        int leftStrength = strength;

        while (leftStrength > 0)
        {
            result.Add(leftStrength);
            leftStrength--;
        }

        return result;
    }
    
    private bool TryGetRandomEffectData(int strength, List<ToggleGlobalEffectData> datas, out ToggleGlobalEffectData data)
    {
        List<ToggleGlobalEffectData> selectedDatas = datas.FindAll(data => data.Quality == strength);
        data = null;
        
        while (selectedDatas.Count > 0)
        {
            int randomIndex = Random.Range(0, selectedDatas.Count);
            ToggleGlobalEffectData selectedData = selectedDatas[randomIndex];
            selectedDatas.RemoveAt(randomIndex);
         
            if (selectedData.IsUnique && TryToFindExistingEffectData(selectedData)) continue;

            if (!CheckIfEffectTagRequirementsAreMet(selectedData)) continue;
            
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

    private bool CheckIfEffectTagRequirementsAreMet<T>(T effectData) where T : GlobalEffectData
    {
        if (!effectData.HasRequiredTags && !effectData.HasBlockTags) return true;
        
        List<GlobalEffectData> createdGlobalEffectDatas = new();
        List<EntityModifcatorTag> buildingsTags = _globalBuildingContainer.GetBuildingTags();
        
        _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().ForEach(item =>
        {
            createdGlobalEffectDatas.AddRange(item.ToggleEffectDatas);
        });
        
        Debug.Log($"Checking requirements for {effectData.name}");
        
        if (effectData.HasRequiredTags) 
        {
            if (!EntityTagRequirementsChecker.RequirementsAreMet(buildingsTags, effectData.RequiredBuildingTags)) return false;
            if (!GlobalEffectTagRequirementsChecker.RequirementsAreMet(createdGlobalEffectDatas, effectData.RequiredGlobalEffectsTags)) return false;
        }
        if (effectData.HasBlockTags)
        {
            if (EntityTagRequirementsChecker.RequirementsAreMet(buildingsTags, effectData.BlockBuildingsTags)) return false;
            if (GlobalEffectTagRequirementsChecker.RequirementsAreMet(createdGlobalEffectDatas, effectData.BlockGlobalEffectTags)) return false;
        }

        
        Debug.Log($"Suckseful Checking requirements for {effectData.name}");
        
        return true;
    }
    
    #endregion
}