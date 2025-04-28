using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public sealed class ItemEffectSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    private ItemModifiersSelectionContainer _temModifiersSelectionContainer;

    private void Awake()
    {
        _temModifiersSelectionContainer = _islandDataContainer.Data.ItemModifiersSelectionContainer;
    }

    #region ModifiersSelection
    
    public List<ToggleGlobalEffectData> GetRandomToggleEffectDatas(int quality, int strength)
    {
        List<ToggleGlobalEffectData> result = new();

        List<ToggleGlobalEffectData> toggleEffectDatas = _temModifiersSelectionContainer.ItemToggleEffectContainer.EffectDatas;

        List<ToggleGlobalEffectData> positiveEffects = toggleEffectDatas.FindAll(effectData => effectData.Quality >= 0);
        List<ToggleGlobalEffectData> negatriveEffects = toggleEffectDatas.FindAll(effectData => effectData.Quality < 0); 

        int positiveStrength = quality + strength;
        int negativeStrength = quality - strength;

        //result.AddRange(GetItemEffectDatas<ToggleGlobalEffectData>(negativeStrength, negatriveEffects));
        result.AddRange(GetItemEffectDatas<ToggleGlobalEffectData>(positiveStrength, positiveEffects));

        return result;   
    }

    public List<RewardGlobalEffectData> GetRandomRewardEffectDatas(int quality, int strength)
    {
        List<RewardGlobalEffectData> result = new();

        List<RewardGlobalEffectData> rewardEffectDatas = _temModifiersSelectionContainer.ItemRewardEffectCotainer.EffectDatas;

        int positiveStrength = quality + strength;
        
        result.AddRange(GetItemEffectDatas<RewardGlobalEffectData>(positiveStrength, rewardEffectDatas));

        return result;
    }

    private List<T> GetItemEffectDatas<T>(int strength, List<T> itemEffectDats) where T : GlobalEffectData
    {
        if (strength == 0) strength = 1;
        
        List<T> result = new();
        Debug.Log($"Trying to find item effect data for {strength}");
        List<int> nonEmptyQualities = PopulateNonEmptyQualityList(strength);
        int leftStrength = strength;
        
        while (leftStrength > 0 && nonEmptyQualities.Count > 0)
        {
            int currentQuality = nonEmptyQualities[Random.Range(0, nonEmptyQualities.Count)];

            if (TryGetRandomEffectData(currentQuality, itemEffectDats, out T effectData))
            {
                itemEffectDats.Remove(effectData);
                result.Add(effectData);

                leftStrength -= currentQuality;
            }
            else
            {
                nonEmptyQualities.Remove(currentQuality);
            }
        }

        return result;
    }

    private bool TryGetRandomEffectData<T>(int quality, List<T> datas, out T data) where T : GlobalEffectData
    {
        List<T> selectedDatas = datas.FindAll(data => data.Quality == quality);
        data = null;
        
        while (selectedDatas.Count > 0)
        {
            int randomIndex = Random.Range(0, selectedDatas.Count);

            T selectedData = selectedDatas[randomIndex];
            
            if (!selectedData.HasAppearanceCondition || _globalEffectFactory.GetAppearanceConditionValue(selectedData))
            {
                data = selectedData;
                return true;
            }

            selectedDatas.RemoveAt(randomIndex);
        }
        
        Debug.Log($"Couldn't find any effect for {quality}");

        return false;
    }

    private List<int> PopulateNonEmptyQualityList(int maxQuality)
    {
        List<int> result = new();
        int currentQuality = maxQuality;

        while (currentQuality > 0)
        {
            result.Add(currentQuality);
            currentQuality--;
        }

        return result;
    }

    #endregion
}