using System.Collections.Generic;
using Combat.Entity.Modificators;
using Combat.EnemySpawnSystem;
using Ligofff.CustomSOIcons;
using CuroLocalization;
using UnityEngine;
using Configs;

public abstract class InspectableConfig : UpdatableScriptableObject
{
    [Header("UI Data")] 
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _localizationKey;
    
    [Space][Header("ArgumentsContainer")]
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [CustomAssetIcon] public Sprite Icon => _icon;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    
    public string GetNameLocalizationKey() => _localizationKey + "_header";
    public string GetName() => GetNameLocalizationKey().Localize();
    public string GetDescription() => ParseByConfig("startTag " + (_localizationKey + "_description").Localize());
    
    public string ParseByConfig(string initialDescription)
    {
        if (this is EntityModificatorConfig)
        {
            EntityModificatorConfig config = (EntityModificatorConfig)this;

            initialDescription = ParseDescriptionDomain(initialDescription, config.DomainConfig);
        }
        
        _argumentsContainer.ArgumentItems.ForEach(argument =>
        {
            switch (argument.ArgumentType)
            {
                case ArgumentType.Float:
                {
                    float value = Mathf.Abs(_argumentsContainer.GetArgument<float>(argument.ArgumentName));
                    
                    initialDescription = ReplaceAllValues(initialDescription, $"{argument.ArgumentName}", value.ToString()); 
                    initialDescription = ReplaceAllValues(initialDescription, $"{argument.ArgumentName}_percent", (value * 100).ToString()); 
                } break;
                case ArgumentType.Int:
                {
                    int value = Mathf.Abs(_argumentsContainer.GetArgument<int>(argument.ArgumentName));
                    
                    initialDescription = ReplaceAllValues(initialDescription, $"{argument.ArgumentName}", value.ToString()); 
                } break;
                case ArgumentType.EntityModificatorData:
                {
                    EntityModificatorConfig modificatorConfig = _argumentsContainer.GetArgument<EntityModificatorConfig>(argument.ArgumentName);
                    
                    initialDescription = modificatorConfig.ParseByConfig(initialDescription);
                } break;
                case ArgumentType.AdditionalEnemyGroup:
                {
                    AdditionalEnemyGroupDataConfig enemyGroupDataConfig = _argumentsContainer.GetArgument<AdditionalEnemyGroupDataConfig>(argument.ArgumentName);

                    initialDescription = ParseByEnemyGroup(enemyGroupDataConfig, initialDescription);
                } break;
            }
        });

        return initialDescription;
        
        string ParseByEnemyGroup(AdditionalEnemyGroupDataConfig additionalEnemyGroupData, string initialText)
        {
            int totalEnemyAmount = 0;
                    
            additionalEnemyGroupData.GroupParts.ForEach(groupPart => totalEnemyAmount += groupPart.GetEnemyAmount(WaveIndexContainer.Instance.GetCurrentWave()));

            initialText = ReplaceAllValues(initialText, "EnemiesAmount", totalEnemyAmount.ToString());
            
            return initialText;
        }
    }

    private string ParseDescriptionDomain(string initialDescription, DomainConfig domainConfig)
    {
        foreach (KeyValuePair<string, ArgumentDataEntry> argumentDataEntry in domainConfig.GetAllArgumentDataEntries())
        {
            string initialValue = argumentDataEntry.Key;
            
            if (initialDescription.Contains(initialValue))
            {
                switch (argumentDataEntry.Value.ArgumentType)
                {
                    case Configs.ArgumentType.Float:
                    {
                        float value = Mathf.Abs(argumentDataEntry.Value.FloatValue);
                    
                        initialDescription = ReplaceAllValues(initialDescription, initialValue, value.ToString()); 
                        initialDescription = ReplaceAllValues(initialDescription, $"{initialValue}%", (value * 100).ToString()); 
                    } break;
                    case Configs.ArgumentType.Int:
                    {
                        int value = Mathf.Abs(argumentDataEntry.Value.IntValue);
                    
                        initialDescription = ReplaceAllValues(initialDescription, initialValue, value.ToString()); 
                    } break;
                    case Configs.ArgumentType.EntityModificatorConfig:
                    {
                        EntityModificatorConfig modificatorConfig = argumentDataEntry.Value.EntityModificatorConfigValue;
                    
                        initialDescription = modificatorConfig.ParseByConfig(initialDescription);
                    } break;
                    case Configs.ArgumentType.AdditionalEnemyGroup:
                    {
                        AdditionalEnemyGroupDataConfig enemyGroupDataConfig = argumentDataEntry.Value.AdditionalEnemyGroupValue;

                        initialDescription = ParseByEnemyGroup(enemyGroupDataConfig, initialDescription);
                    } break;
                }
            }
        }
        
        return initialDescription;
        
        string ParseByEnemyGroup(AdditionalEnemyGroupDataConfig additionalEnemyGroupData, string initialText)
        {
            int totalEnemyAmount = 0;
                    
            additionalEnemyGroupData.GroupParts.ForEach(groupPart => totalEnemyAmount += groupPart.GetEnemyAmount(WaveIndexContainer.Instance.GetCurrentWave()));

            initialText = ReplaceAllValues(initialText, "EnemiesAmount", totalEnemyAmount.ToString());
            
            return initialText;
        }
    }
    
    private string ReplaceAllValues(string initialString, string initialValue, string endValue)
    {
        initialValue = '{' + initialValue + '}';
        
        while (initialString.Contains(initialValue))
        {
            initialString = initialString.Replace(initialValue, endValue);
        }
        
        return initialString;
    }
}