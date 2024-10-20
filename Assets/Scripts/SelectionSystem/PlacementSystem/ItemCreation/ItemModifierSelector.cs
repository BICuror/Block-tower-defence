using Zenject;
using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using WorldGeneration;
using System.Linq;

public class ItemModifierSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private DiContainer _diContainer;
    private ItemModifiersSelectionContainer _temModifiersSelectionContainer;

    private void Awake()
    {
        _temModifiersSelectionContainer = _islandDataContainer.Data.ItemModifiersSelectionContainer;
    }

    #region Modifiers selection
    public List<ItemPropertyData> GetProperties(int quality, int strength)
    {
        List<ItemPropertyData> result = new();

        ItemPropertyContainer propertyContainer = _temModifiersSelectionContainer.ItemPropertyContainer;

        List<ItemPropertyData> positiveProperties = new List<ItemPropertyData>(propertyContainer.PositiveModifiers);
        List<ItemPropertyData> negativeProperties = new List<ItemPropertyData>(propertyContainer.NegativeModifiers); 

        int positiveStrength = quality + strength;
        int negativeStrength = quality - strength;

        result.AddRange(GetItemModifiers<ItemPropertyData>(negativeStrength, negativeProperties));

        return result;   
    }

    public List<ItemRewardData> GetRewards(int quality, int strength)
    {
        List<ItemRewardData> result = new();

        ItemRewardContainer rewardContainer = _temModifiersSelectionContainer.ItemRewardContainer;
        
        List<ItemRewardData> positiveRewards = new List<ItemRewardData>(rewardContainer.PositiveModifiers);
        List<ItemRewardData> negativeRewards = new List<ItemRewardData>(rewardContainer.NegativeModifiers);

        int positiveStrength = quality + strength;
        int negativeStrength = quality - strength;
        
        result.Add(GetRandomItemModifier<ItemRewardData>(positiveStrength, positiveRewards));

        return result;
    }

    private List<T> GetItemModifiers<T>(int strength, List<T> itemModifiers) where T : ItemModifierData
    {
        List<T> result = new();

        while (strength > 0)
        {
            int nextWeight = Random.Range(1, strength);
            strength -= nextWeight;

            result.Add(GetRandomItemModifier<T>(strength, itemModifiers));
        }

        return result;
    }

    private T GetRandomItemModifier<T>(int quality, List<T> modifiers) where T: ItemModifierData
    {
        List<T> selectedDatas = modifiers.Where((data) => data.Quality == quality).ToList();

        while (selectedDatas.Count > 0)
        {
            int randomIndex = Random.Range(0, selectedDatas.Count);
            
            if (GetAppeanceConditionValue(selectedDatas[randomIndex]))
            {
                return selectedDatas[randomIndex];
            }
            else 
            {
                selectedDatas.RemoveAt(randomIndex);
            }
        }

        Debug.LogError($"Not enough modifiers with quality of {quality}");
        return null;
    }

    private bool GetAppeanceConditionValue(ItemModifierData modifierData)
    {
        if (modifierData.HasApperanceCondition == false) return true;
    
        ItemModifierApperanceCondition condition = (ItemModifierApperanceCondition)Activator.CreateInstance(modifierData.ApperanceConditionTypeName);

        _diContainer.Inject(condition);

        return condition.GetValue();
    }
    #endregion
}