using UnityEngine;

[CreateAssetMenu(fileName = "CrystalSpawnSettings", menuName = "Block Tower Defence/CrystalSpawnSettings", order = 0)]

public sealed class CrystalSpawnSettings : ScriptableObject 
{
    [SerializeField] private SpecialCrystalSpawnSetting[] _spawnSetting;
    public SpecialCrystalSpawnSetting[] SpawnSettings => _spawnSetting;

    [System.Serializable] public struct SpecialCrystalSpawnSetting
    {
        public CrystalType Type;
        [Range(0f, 1f)] public float SpawnChanse;
    }
}

public enum CrystalType
{
    Building,
    Wave,
    Upgrade
}
