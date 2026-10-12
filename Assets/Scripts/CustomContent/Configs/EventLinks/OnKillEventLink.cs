using EventLinks;
using System;
using Combat;

public sealed class OnKillEventLink : EventLinkWithParameter<CombatEntity>
{
    protected override void SubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ValueModifierContainer.EntityKilled += action;
    }

    protected override void UnsubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Owner.ValueModifierContainer.EntityKilled -= action;
    }
}