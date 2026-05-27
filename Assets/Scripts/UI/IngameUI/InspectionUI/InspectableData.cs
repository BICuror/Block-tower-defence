using System.Collections.Generic;
using Ligofff.CustomSOIcons;
using CuroLocalization;
using UnityEngine;

public abstract class InspectableData : ScriptableObject
{
    [Header("UI Data")] 
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _localizationKey;
    
    [Space] [Header("ArgumentsContainer")]
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [CustomAssetIcon] public Sprite Icon => _icon;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    
    public string GetNameLocalizationKey() => _localizationKey + "_header";
    public string GetName() => GetNameLocalizationKey().Localize();
    public string GetDescription() => ParseDescription("startTag " + (_localizationKey + "_description").Localize());
    
    private string ParseDescription(string initialDescription)
    {
        if (this is EntityModificatorData)
        {
            EntityModificatorData data = (EntityModificatorData)this;
            
            initialDescription = ParseByStatInitializer(data.StatInitializers, initialDescription);
            initialDescription = ParseByStatChange(data.StatChanges, initialDescription);
        }
        
        if (this is GlobalStatChangeToggleEffectData)
        {
            GlobalStatChangeToggleEffectData data = (GlobalStatChangeToggleEffectData)this;
            
            initialDescription = ParseByStatChange(data.StatChanges, initialDescription);
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
                    EntityModificatorData modificatorData = _argumentsContainer.GetArgument<EntityModificatorData>(argument.ArgumentName);
                    
                    initialDescription = modificatorData.ParseDescription(initialDescription);

                    initialDescription = ParseByStatChange(modificatorData.StatChanges, initialDescription);
                    initialDescription = ParseByStatInitializer(modificatorData.StatInitializers, initialDescription);
                } break;
                case ArgumentType.AdditionalEnemyGroup:
                {
                    AdditionalEnemyGroupData enemyGroupData = _argumentsContainer.GetArgument<AdditionalEnemyGroupData>(argument.ArgumentName);

                    initialDescription = ParseByEnemyGroup(enemyGroupData, initialDescription);
                } break;
            }
        });

        return initialDescription;

        string ParseByStatChange(List<StatChange> statChanges, string initialText)
        {
            statChanges.ForEach(statChange =>
            {
                initialText = ReplaceAllValues(initialText, $"{statChange.StatData.GetStatType().Name}_flat", Mathf.Abs(statChange.FlatChange).ToString());
                initialText = ReplaceAllValues(initialText, $"{statChange.StatData.GetStatType().Name}_mult", Mathf.Abs(statChange.MultiplierChange * 100).ToString());
            });

            return initialText;
        }

        string ParseByStatInitializer(List<StatInitializer> initializers, string initialText)
        {
            initializers.ForEach(statInitializer =>
            {
                initialText = ReplaceAllValues(initialText, $"{statInitializer.StatData.GetStatType().Name}", Mathf.Abs(statInitializer.DefaultValue).ToString());
            });

            return initialText;
        }
        
        string ParseByEnemyGroup(AdditionalEnemyGroupData additionalEnemyGroupData, string initialText)
        {
            int totalEnemyAmount = 0;
                    
            additionalEnemyGroupData.GroupParts.ForEach(groupPart => totalEnemyAmount += groupPart.GetAmount(WaveIndexContainer.Instance.GetCurrentWave()));

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