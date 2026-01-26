using UnityEngine;
using Cashing;
using Combat;

public sealed class HaveInvincibilityWhenNotHoveredOver : EntityObjectModifier
{
    [SerializeField] private HoverableObject _hoverableObject;
    [Cached] private EntityEffectManager _entityEffectManager;

    private void Start()
    {
        _entityEffectManager.TryApplyEffect(typeof(InvincibilityEffect), 1);

        _hoverableObject.HoverEntered.AddListener(RemoveEffect);
        _hoverableObject.HoverExited.AddListener(AddEffect);
    }

    private void AddEffect() => _entityEffectManager.TryApplyEffect(typeof(InvincibilityEffect), 1);
    private void RemoveEffect() => _entityEffectManager.RemoveEffect(typeof(InvincibilityEffect), 1);
}