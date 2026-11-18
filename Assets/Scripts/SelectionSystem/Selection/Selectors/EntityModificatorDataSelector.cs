using System.Collections.Generic;
using Combat.Entity.Modificators;
using UnityEngine;
using System.Linq;
using Zenject;
using Combat;
using System;
using Random = UnityEngine.Random;

public sealed class EntityModificatorDataSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [SerializeField] [Range(0f, 100f)] private int _additionalChansePerSameTag;
    [SerializeField] private List<EntityModificatorRarityDrop> _modificatorRarityDrops;
    private int _currentSelectionIndex;
    
    public List<EntityModificatorConfig> GetRandomEntityEffectDatas(BuildingEntity entity, int amount)
    {
        List<EntityModificatorConfig> resultEffectDatas = new();
        
        List<EntityModificatorConfig> allEffectDatas = _islandDataContainer.Data.EntityModificatorConfigContainer.EntityModificatorDataList.FindAll(modificator => UpgradeCanAppear(modificator, entity));
        
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
            
            List<EntityModificatorConfig> modificatorsOfRarity = allEffectDatas.Where(modificatorData => modificatorData.Rarity == randomRarity).ToList();

            EntityModificatorConfig randomModificator = GetRandomEntityModificatorDataFromGroup(entity, modificatorsOfRarity);
            
            resultEffectDatas.Add(randomModificator);
            allEffectDatas.Remove(randomModificator);
        } 
        
        _currentSelectionIndex++;

        return resultEffectDatas;
    }

    private bool UpgradeCanAppear(EntityModificatorConfig modificatorConfig, BuildingEntity entity)
    {
        return CheckBuildingEntityTagRequirements(modificatorConfig, entity) && 
               CheckStacksRequirements(modificatorConfig, entity) && 
               CheckTagRequirements(modificatorConfig, entity) && 
               CheckExistingRequiredUpgradesRequirement(modificatorConfig, entity);
    }

    #region BuildingEntityTagCheck

    private bool CheckBuildingEntityTagRequirements(EntityModificatorConfig modificatorConfig, BuildingEntity entity)
    {
        if (modificatorConfig.BuildingTagsInclude)
        {
            return modificatorConfig.BuildingEntityTypeTags.Contains(entity.BuildingEntityType);
        }
        
        return !modificatorConfig.BuildingEntityTypeTags.Contains(entity.BuildingEntityType);
    }

    #endregion
    
    #region UniquieTagCheck

    private bool CheckStacksRequirements(EntityModificatorConfig modificatorConfig, BuildingEntity entity)
    {
        if (modificatorConfig.HasStacks)
        {
            return entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Count(appliedModificatorData => appliedModificatorData == modificatorConfig) < modificatorConfig.MaxStacks;
        }

        return true;
    }

    #endregion

    #region TagRequirementsCheck

    private bool CheckTagRequirements(EntityModificatorConfig modificatorConfig, BuildingEntity entity)
    {
        if (!modificatorConfig.HasRequiredTags && !modificatorConfig.HasBlockTags) return true;

        List<EntityModifcatorTag> ownerTags = entity.ComponentsContainer.Get<EntityModificatorsContainer>().GetAppliedTags();
        ownerTags.AddRange(entity.DefaultBuildingTags);
        
        List<EntityModifcatorTag> otherTags = _globalBuildingContainer.GetBuildingTags(entity);
        
        if (modificatorConfig.HasRequiredTags)
        {
            if (modificatorConfig.ReqiredOwnerTags.Count > 0 && !EntityTagRequirementsChecker.RequirementsAreMet(ownerTags, modificatorConfig.ReqiredOwnerTags)) return false;
            if (modificatorConfig.ReqiredOtherEntityTags.Count > 0 && !EntityTagRequirementsChecker.RequirementsAreMet(otherTags, modificatorConfig.ReqiredOtherEntityTags)) return false;
        }

        if (modificatorConfig.HasBlockTags)
        {
            if (modificatorConfig.BlockOwnerTags.Count > 0 && EntityTagRequirementsChecker.RequirementsAreMet(ownerTags, modificatorConfig.BlockOwnerTags)) return false;
            if (modificatorConfig.BlockOtherEntityTags.Count > 0 && EntityTagRequirementsChecker.RequirementsAreMet(otherTags, modificatorConfig.BlockOtherEntityTags)) return false;
        }
        
        return true;
    }
    
    #endregion
    
    #region ExistingUpgradesRequirementsCheck

    private bool CheckExistingRequiredUpgradesRequirement(EntityModificatorConfig modificatorConfig, BuildingEntity entity)
    {
        if (!modificatorConfig.HasRequiredUpgrades) return true;
        
        for (int i = 0; i < modificatorConfig.RequiredUpgrades.Count; i++)
        {
            if (!entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Contains(modificatorConfig.RequiredUpgrades[i])) return false;
        }
        
        return true;
    }
    
    #endregion
    
    #region RaritySelection
    
    private List<EntityModificatorRarityDrop> GetAvailableRarityDrops(List<EntityModificatorConfig> availableModificators)
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
    
    private EntityModificatorConfig GetRandomEntityModificatorDataFromGroup(BuildingEntity buildingEntity, List<EntityModificatorConfig> possibleModificatorGroup)
    {
        List<EntityModifcatorTag> entityModifcatorTags = buildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().GetAppliedTags();
        
        Dictionary<EntityModifcatorTag, int> entityModifcatorTagCounts = new();
        
        entityModifcatorTags.ForEach(modificatorTag =>
        {
            if (entityModifcatorTagCounts.ContainsKey(modificatorTag)) entityModifcatorTagCounts[modificatorTag]++;
            else entityModifcatorTagCounts.Add(modificatorTag, 1);
        });
        
        List<EntityModificatorConfig> unobtainedModificatorDataList = possibleModificatorGroup.Except(buildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators).ToList();
        
        foreach (EntityModifcatorTag entityModifcatorTag in entityModifcatorTagCounts.Keys)
        {
            if (entityModifcatorTagCounts[entityModifcatorTag] * _additionalChansePerSameTag > Random.Range(0, 100))
            {
                if (unobtainedModificatorDataList.Exists(modificator => modificator.Tags.Contains(entityModifcatorTag)))
                {
                    List<EntityModificatorConfig> tagEntityModificatorDataList = unobtainedModificatorDataList.FindAll(modificator => modificator.Tags.Contains(entityModifcatorTag));    
                    
                    return tagEntityModificatorDataList[Random.Range(0, tagEntityModificatorDataList.Count)];
                }
            }
        }
        
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