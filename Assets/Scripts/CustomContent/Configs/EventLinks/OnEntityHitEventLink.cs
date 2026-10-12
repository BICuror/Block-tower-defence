using EventLinks;
using System;
using Combat;

public sealed class OnEntityHitEventLink : EventLinkWithParameter<CombatEntity>
{
    protected override void SubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ValueModifierContainer.EnemyHit += action;
    }

    protected override void UnsubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ValueModifierContainer.EnemyHit -= action;
    }
}