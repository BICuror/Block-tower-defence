using UnityEngine;
using System;
using Combat;

public sealed class ChangeStatPerEntityInArea : EntityModificator
{
    private StatModifier _statModifier = new();
    private Type _statType;
    private float _flatChangePerEntity;
    private float _flatChangeMax;
    private float _multChangePerEntity;
    private float _multChangeMax;

    public override void Enable()
    {
        _statType = Type.GetType(Args.GetArgument<string>("StatType"));

        _flatChangePerEntity = Args.GetArgument<float>("FlatChangePerEntity");
        _flatChangeMax = Args.GetArgument<float>("FlatChangeMax");
        _multChangePerEntity = Args.GetArgument<float>("MultChangePerEntity");
        _multChangeMax = Args.GetArgument<float>("MultChangeMax");

        Entity.StatContainer.Get(_statType).AddStatModifier(_statModifier);

        Entity.ComponentsContainer.Get<AreaEntityDetector>().AddedItem += OnAreaEntityDetectorUpdated;
        Entity.ComponentsContainer.Get<AreaEntityDetector>().RemovedItem += OnAreaEntityDetectorUpdated;
    }

    private void OnAreaEntityDetectorUpdated(CombatEntity _) => UpdateStatModifier();

    private void UpdateStatModifier()
    {
        int entitiesInArea = Entity.ComponentsContainer.Get<AreaEntityDetector>().Count;

        float flatChange = Mathf.Clamp(entitiesInArea * _flatChangePerEntity, -_flatChangeMax, _flatChangeMax);
        _statModifier.SetFlat(flatChange);

        float multChange = Mathf.Clamp(entitiesInArea * _multChangePerEntity, -_multChangeMax, _multChangeMax);
        _statModifier.SetMultiplier(multChange);
    }

    public override void Disable()
    {
        Entity.StatContainer.Get(_statType).RemoveStatModifier(_statModifier);

        Entity.ComponentsContainer.Get<AreaEntityDetector>().AddedItem -= OnAreaEntityDetectorUpdated;
        Entity.ComponentsContainer.Get<AreaEntityDetector>().RemovedItem -= OnAreaEntityDetectorUpdated;
    }
}