using TMPEffects.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayerSettingsServiceConfig", menuName = "PhysicLayers/LayerSettingsServiceConfig")]

public sealed class LayerSettingsServiceConfig : ScriptableObject
{
    [SerializeField] private SerializedDictionary<LayerSettingType, LayerSetting> _layerSettingDictionary;

    public Dictionary<LayerSettingType, LayerSetting> LayerSettingDictionary => _layerSettingDictionary;
}

public enum LayerSettingType
{
    None = 0,
    SolidObjects = 1,
    SolidTerrain = 2,
    Buildings = 3,
    Enemies = 4,
    WaterAndTerrain = 5,
    InspectableObjects = 6,
    AnyTerrain = 7,
    RoadTerrain = 8,
    Townhall = 9,
    NonstackableCreatedItems = 10,
    BlockingDecoration = 11,
    SolidTerrainAndRoad = 12,
    DecorationExclusionLayer = 13,
}