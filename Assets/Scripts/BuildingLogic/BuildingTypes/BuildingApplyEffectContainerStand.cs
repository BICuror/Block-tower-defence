using System.Collections.Generic;
using UnityEngine;

public sealed class BuildingApplyEffectContainerStand : MonoBehaviour
{
    [SerializeField] private ApplyEffectContainerAreaScaner _applyEffectContainerAreaScaner;

    [SerializeField] private Effect[] _applyEffects;

    private void Awake()
    {
       _applyEffectContainerAreaScaner.PlacedComponentAdded.AddListener(AddEffectToContainer);
       _applyEffectContainerAreaScaner.PlacedComponentRemoved.AddListener(RemoveEffectFromContainer);
    }

    private void AddEffectToContainer(ApplyEffectContainer container)
    {
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            container.AddEffect(_applyEffects[i]);
        }
    }

    private void RemoveEffectFromContainer(ApplyEffectContainer container)
    {
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            container.RemoveEffect(_applyEffects[i]);
        }
    }
}
