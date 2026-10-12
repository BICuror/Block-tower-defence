using EventLinks;
using System;
using Combat;

public sealed class OnEntityExitMainAreaEventLink : EventLinkWithParameter<CombatEntity>
{
    protected override void SubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ComponentsContainer.Get<AreaEntityDetector>().RemovedItem += action;
    }

    protected override void UnsubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ComponentsContainer.Get<AreaEntityDetector>().RemovedItem -= action;
    }
}