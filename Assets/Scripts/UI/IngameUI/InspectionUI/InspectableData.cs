using Ligofff.CustomSOIcons;
using CuroLocalization;
using UnityEngine;

public abstract class InspectableData : ScriptableObject
{
    [Header("UI Data")] 
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _localizationKey;
    [SerializeField] private string _modificatorName;
    [TextArea] [SerializeField] private string _modificatorDescription;
    
    [Space] [Header("ArgumentsContainer")]
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [CustomAssetIcon] public Sprite Icon => _icon;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public string GetName() => (_localizationKey + "_header").Localize();
    public string GetDescription() => ParseDescription((_localizationKey + "_description").Localize());
    
    private string ParseDescription(string initialDescription)
    {
        _argumentsContainer.ArgumentItems.ForEach(argument =>
        {
            switch (argument.ArgumentType)
            {
                case ArgumentType.Float:
                {
                    float value = _argumentsContainer.GetArgument<float>(argument.ArgumentName);
                    
                    initialDescription = ReplaceAllValues(initialDescription, $"{argument.ArgumentName}", (value * 100).ToString()); 
                } break;
                case ArgumentType.Int:
                {
                    int value = _argumentsContainer.GetArgument<int>(argument.ArgumentName);
                    
                    initialDescription = ReplaceAllValues(initialDescription, $"{argument.ArgumentName}", value.ToString()); 
                } break;
                case ArgumentType.EntityModificatorData:
                {
                    EntityModificatorData modificatorData = _argumentsContainer.GetArgument<EntityModificatorData>(argument.ArgumentName);
                    
                    initialDescription = modificatorData.ParseDescription(initialDescription);

                    if (modificatorData is EntityModificatorStatChangeData)
                    {
                        EntityModificatorStatChangeData statChangeData = modificatorData as EntityModificatorStatChangeData;
                        
                        statChangeData.StatChanges.ForEach(statChange =>
                        {
                            initialDescription = ReplaceAllValues(initialDescription, $"{statChange.StatData.GetStatType().Name}_flat", statChange.FlatChange.ToString());
                            initialDescription = ReplaceAllValues(initialDescription, $"{statChange.StatData.GetStatType().Name}_mult", Mathf.Abs(statChange.MultiplierChange * 100).ToString());
                        });
                    }
                } break;
                case ArgumentType.AdditionalEnemyGroup:
                {
                    AdditionalEnemyGroupData enemyGroupData = _argumentsContainer.GetArgument<AdditionalEnemyGroupData>(argument.ArgumentName);

                    int totalEnemyAmount = 0;
                    
                    enemyGroupData.GroupParts.ForEach(groupPart => totalEnemyAmount += groupPart.GetAmount(WaveIndexContainer.Instance.GetCurrentWave()));

                    initialDescription = ReplaceAllValues(initialDescription, "EnemiesAmount", totalEnemyAmount.ToString());
                } break;
            }
        });

        return initialDescription;
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