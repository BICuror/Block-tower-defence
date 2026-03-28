using Cysharp.Threading.Tasks;
using CuroSettings;
using UnityEngine;

public sealed class DamageNumberDisplayManager : MonoBehaviour
{
    private static DamageNumberDisplayManager _instance;
    public static DamageNumberDisplayManager Instance => _instance;
    
    [SerializeField] private DamageNumberDisplay _damageNumberDisplayPrefab;
    private ObjectPool<DamageNumberDisplay> _displayPool;
    private BoolSetting _damageIndicatorsEnabled;
    
    private void Awake()
    {
        _instance = this;
        _displayPool = new ObjectPool<DamageNumberDisplay>(_damageNumberDisplayPrefab, 10);
        _damageIndicatorsEnabled = SettingsContainer.GetSetting<BoolSetting>(SettingsEnum.EnableDamageIndicators);
    }

    public void DisplayDamageNumber(float damage, Vector3 position, DamageVisualsType damageVisualsType)
    {
        if (!_damageIndicatorsEnabled.Value) return;
        
        _displayPool.GetNextPooledObject().DisplayDamageNumber(damage, position, damageVisualsType).Forget();
    }
}