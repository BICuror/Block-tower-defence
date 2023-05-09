using System.Collections.Generic;
using UnityEngine;

public sealed class BuildingApplyEffectContainerStand : MonoBehaviour
{
    [SerializeField] private BuildingsAreaScaner _buildingAreaScaner;

    [SerializeField] private Effect[] _applyEffects;

    private void Start()
    {
        _buildingAreaScaner.ErectedBuildingAdded.AddListener(TryAddEffectToContainer);
        _buildingAreaScaner.ErectedBuildingRemoved.AddListener(TryRemoveEffectFromContainer);
    }

    private void TryAddEffectToContainer(Building building)
    {
        if (building.gameObject.TryGetComponent(out ApplyEffectContainer container))
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                container.AddEffect(_applyEffects[i]);
            }
        }
    }

    private void TryRemoveEffectFromContainer(Building building)
    {
        if (building.gameObject.TryGetComponent(out ApplyEffectContainer container))
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                container.RemoveEffect(_applyEffects[i]);
            }
        }
    }
    
    private void OnDestroy() => RemoveAllEffects();

    private void RemoveAllEffects()
    {
        List<Building> building = _buildingAreaScaner.GetErectedBuildings();

        for (int i = 0; i < building.Count; i++)
        {
            TryRemoveEffectFromContainer(building[i]);
        }
    }
}
