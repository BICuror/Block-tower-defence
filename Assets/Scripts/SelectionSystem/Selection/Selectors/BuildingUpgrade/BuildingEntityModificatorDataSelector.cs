using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Combat;
using System;

using Random = UnityEngine.Random;

public sealed class BuildingEntityModificatorDataSelector : MonoBehaviour
{
    [SerializeField] [Range(0f, 100f)] private int _additionalChansePerSameTag;
    [SerializeField] private List<EntityModificatorRarityDrop> _modificatorRarityDrops;
    private int _currentSelectionIndex;
    
    public List<EntityModificatorData> GetRandomEntityEffectDatas(BuildingEntity entity, int amount)
    {
        List<EntityModificatorData> resultEffectDatas = new();
        List<EntityModificatorData> allEffectDatas = entity.ComponentsContainer.Get<EntityModificatorsContainer>().AvailableModificators;
        List<EntityModifcationRarity> droppedRarities = new();

        for (int i = 0; i < amount; i++)
        {
            List<EntityModificatorRarityDrop> availableRarities = GetAvailableRarityDrops(allEffectDatas);
            EntityModifcationRarity randomRarity = GetRandomModificatorRarity(availableRarities);
            droppedRarities.Add(randomRarity);

            //Guarantees at least one legendary drop per selection every second upgrade selection
            if (amount - 1 == i && !droppedRarities.Contains(EntityModifcationRarity.Legendary) && _currentSelectionIndex % 2 == 0)
            {
                randomRarity = GetHighestMofigicatorRarity(availableRarities);
            }
            
            List<EntityModificatorData> modificatorsOfRarity = allEffectDatas.Where(modificatorData => modificatorData.Rarity == randomRarity).ToList();
            List<EntityModificatorData> tagSortedModificators = TrySortModificatorsByTag(modificatorsOfRarity, entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators);

            EntityModificatorData randomModificator = GetRandomEntityModificatorDataFromGroup(tagSortedModificators);
            
            resultEffectDatas.Add(randomModificator);
            allEffectDatas.Remove(randomModificator);
        }
        
        _currentSelectionIndex++;

        return resultEffectDatas;
    }

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

    private EntityModifcationRarity GetHighestMofigicatorRarity(List<EntityModificatorRarityDrop> availableModificatorRarityDrops)
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
        
        float currentChanse = Random.Range(0f, totalChanse);
        
        for (int i = 0; i < _additionalChansePerSameTag; i++)
        {
            if (availableModificatorRarityDrops[i].DropChance >= currentChanse)
            {
                return availableModificatorRarityDrops[i].Rarity;
            }
        }
        
        throw new Exception("No modificator rarity available");
    }

    private List<EntityModificatorData> TrySortModificatorsByTag(List<EntityModificatorData> availableModificators, List<EntityModificatorData> appliedModificatorDatas)
    {
        Dictionary<EntityModifcatorTag, int> appliedTags = GetAppliedTags(appliedModificatorDatas);
        List<EntityModifcatorTag> tags = appliedTags.Keys.ToList();
        
        for (int i = 0; i < tags.Count; i++)
        {
            EntityModifcatorTag tag = tags[i];

            if (Random.Range(0f, 100f) < _additionalChansePerSameTag * appliedTags[tag])
            {
                return availableModificators.Where(modificator => modificator.Tags.Contains(tag)).ToList();
            }
        }

        return availableModificators;
    }

    private Dictionary<EntityModifcatorTag, int> GetAppliedTags(List<EntityModificatorData> appliedModificatorDatas)
    {
        Dictionary<EntityModifcatorTag, int> appliedTags = new();
        
        appliedModificatorDatas.ForEach(modificatorData =>
        {
            modificatorData.Tags.ForEach(tag =>
            {
                if (appliedTags.ContainsKey(tag))
                {
                    appliedTags[tag]++;
                }
                else
                {
                    appliedTags.Add(tag, 1);
                }
            });
        });
        
        return appliedTags;
    }
    
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