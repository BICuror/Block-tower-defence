using System.Collections.Generic;
using UnityEngine;

public sealed class BuildingEffectStand : MonoBehaviour
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
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            building.gameObject.GetComponent<EntityEffectManager>().ApplyEffect(_applyEffects[i]);
        }
    }

    private void TryRemoveEffectFromContainer(Building building)
    {
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            building.gameObject.GetComponent<EntityEffectManager>().RemoveEffect(_applyEffects[i]);
        }
    }
    
    private void OnDestroy() => RemoveAllEffects();

    public void RemoveAllEffects()
    {
        List<Building> building = _buildingAreaScaner.GetErectedBuildings();

        for (int i = 0; i < building.Count; i++)
        {
            TryRemoveEffectFromContainer(building[i]);
        }
    }
}
