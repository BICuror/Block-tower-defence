using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;
using Combat;
using System;

using Random = UnityEngine.Random;

public sealed class EntityModificatorDataSelector : MonoBehaviour
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [SerializeField] [Range(0f, 100f)] private int _additionalChansePerSameTag;
    [SerializeField] private List<EntityModificatorRarityDrop> _modificatorRarityDrops;
    private int _currentSelectionIndex;
    
    public List<EntityModificatorData> GetRandomEntityEffectDatas(BuildingEntity entity, int amount)
    {
        List<EntityModificatorData> resultEffectDatas = new();
        
        List<EntityModificatorData> allEffectDatas = entity.ComponentsContainer.Get<EntityModificatorsContainer>().AvailableModificators.FindAll(modificator =>
        {
            return CheckStacksRequirements(modificator, entity) && CheckTagRequirements(modificator, entity);
        });
        
        List<EntityModifcationRarity> droppedRarities = new();

        for (int i = 0; i < amount; i++)
        {
            List<EntityModificatorRarityDrop> availableRarities = GetAvailableRarityDrops(allEffectDatas);
            EntityModifcationRarity randomRarity = GetRandomModificatorRarity(availableRarities);
            droppedRarities.Add(randomRarity);

            //Guarantees at least one legendary drop per selection every second upgrade selection
            if (amount - 1 == i && !droppedRarities.Contains(EntityModifcationRarity.Legendary) && _currentSelectionIndex % 2 == 0)
            {
                randomRarity = GetHighestModificatorRarity(availableRarities);
            }
            
            List<EntityModificatorData> modificatorsOfRarity = allEffectDatas.Where(modificatorData => modificatorData.Rarity == randomRarity).ToList();

            EntityModificatorData randomModificator = GetRandomEntityModificatorDataFromGroup(modificatorsOfRarity);
            
            resultEffectDatas.Add(randomModificator);
            allEffectDatas.Remove(randomModificator);
        } 
        
        _currentSelectionIndex++;

        return resultEffectDatas;
    }

    #region UniquieTagCheck

    private bool CheckStacksRequirements(EntityModificatorData modificatorData, BuildingEntity entity)
    {
        if (modificatorData.HasStacks)
        {
            return entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Count(appliedModificatorData => appliedModificatorData == modificatorData) <= modificatorData.MaxStacks;
        }

        return true;
    }

    #endregion

    #region TagRequirementsCheck

    private bool CheckTagRequirements(EntityModificatorData modificatorData, BuildingEntity entity)
    {
        if (!modificatorData.HasRequiredTags && !modificatorData.HasBlockTags) return true;

        List<EntityModifcatorTag> ownerTags = entity.ComponentsContainer.Get<EntityModificatorsContainer>().GetAppliedTags();
        List<EntityModifcatorTag> otherTags = _globalBuildingContainer.GetBuildingTags(entity);

        Debug.Log($"Checking requirements for {modificatorData.name}");
        
        if (modificatorData.HasRequiredTags)
        {
            if (modificatorData.ReqiredOwnerTags.Count > 0 && !EntityTagRequirementsChecker.RequirementsAreMet(ownerTags, modificatorData.ReqiredOwnerTags)) return false;
            if (modificatorData.ReqiredOtherEntityTags.Count > 0 && !EntityTagRequirementsChecker.RequirementsAreMet(otherTags, modificatorData.ReqiredOtherEntityTags)) return false;
        }

        if (modificatorData.HasBlockTags)
        {
            if (modificatorData.BlockOwnerTags.Count > 0 && EntityTagRequirementsChecker.RequirementsAreMet(ownerTags, modificatorData.BlockOwnerTags)) return false;
            if (modificatorData.BlockOtherEntityTags.Count > 0 && EntityTagRequirementsChecker.RequirementsAreMet(otherTags, modificatorData.BlockOtherEntityTags)) return false;
        }
        
        Debug.Log($"Suckseful Checking requirements for {modificatorData.name}");
        
        return true;
    }
    
    #endregion

    #region RaritySelection
    
    private List<EntityModificatorRarityDrop> GetAvailableRarityDrops(List<EntityModificatorData> availableModificators)
    {
        List<EntityModifcationRarity> allRarities = Enum.GetValues(typeof(EntityModifcationRarity)).Cast<EntityModifcationRarity>().ToList();
        
        List<EntityModificatorRarityDrop> resultRarityDrops = new();
        
        allRarities.ForEach(rarity =>
        {
            if (availableModificators.Exists(modificatorData => modificatorData.Rarity == rarity))
            {
                resultRarityDrops.Add(_modificatorRarityDrops.Find(rarityDrop => rarityDrop.Rarity == rarity));
            }
        });

        return resultRarityDrops;
    }

    private EntityModifcationRarity GetHighestModificatorRarity(List<EntityModificatorRarityDrop> availableModificatorRarityDrops)
    {
        List<EntityModifcationRarity> allRarities = Enum.GetValues(typeof(EntityModifcationRarity)).Cast<EntityModifcationRarity>().ToList();

        for (int i = allRarities.Count - 1; i >= 0; i--)
        {
            if (availableModificatorRarityDrops.Exists(rarityDrop => rarityDrop.Rarity == allRarities[i]))
            {
                return allRarities[i];
            }
        }
        
        throw new Exception("No modificator rarity available");
    }
    
    private EntityModifcationRarity GetRandomModificatorRarity(List<EntityModificatorRarityDrop> availableModificatorRarityDrops)
    {
        float totalChanse = 0;
        
        availableModificatorRarityDrops.ForEach(group => totalChanse += group.DropChance);
        
        float currentChanse = Random.Range(0, totalChanse);
        
        for (int i = 0; i < availableModificatorRarityDrops.Count; i++)
        {
            if (availableModificatorRarityDrops[i].DropChance >= currentChanse)
            {
                return availableModificatorRarityDrops[i].Rarity;
            }
            
            currentChanse -= availableModificatorRarityDrops[i].DropChance;
        }
        
        throw new Exception("No modificator rarity available");
    }
    
    #endregion
    
    private EntityModificatorData GetRandomEntityModificatorDataFromGroup(List<EntityModificatorData> possibleModificatorGroup)
    {
        return possibleModificatorGroup[Random.Range(0, possibleModificatorGroup.Count)];
    }

    [Serializable] private sealed class EntityModificatorRarityDrop
    {
        [SerializeField] private EntityModifcationRarity _rarity;
        [SerializeField] [Range(0f, 100f)] private float _dropChance;
        
        public EntityModifcationRarity Rarity => _rarity;
        public float DropChance => _dropChance;
    }
}