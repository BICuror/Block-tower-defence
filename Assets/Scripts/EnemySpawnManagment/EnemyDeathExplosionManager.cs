using UnityEngine;
using Cashing;
using Combat;

public sealed class EnemyDeathExplosionManager : MonoBehaviour
{
    [Cached] private EntityHealth _entityHealth;
    [SerializeField] private VisualEffectHandler _explosionVisualEffectHandler;
    [SerializeField] private VisualEffectHandler _splatVisualEffectHandler;
    [SerializeField] private LayerSetting _anyTerrainLayerSetting;
    
    private void Start()
    {
        _entityHealth.EntityDied += PlayExplosionVFX;
    }

    private void PlayExplosionVFX(CombatEntity enemyObject)
    {
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        if (TileMap.HasTile(roundedPosition, _anyTerrainLayerSetting))
        {
            _explosionVisualEffectHandler.PlayBurstEffectAndForget();
            _splatVisualEffectHandler.PlayBurstEffectAndForget();
            
            _splatVisualEffectHandler.transform.position = new Vector3(_explosionVisualEffectHandler.transform.position.x, TileMap.GetHitInfo(roundedPosition, _anyTerrainLayerSetting).point.y, _explosionVisualEffectHandler.transform.position.z);
        }
    }
}