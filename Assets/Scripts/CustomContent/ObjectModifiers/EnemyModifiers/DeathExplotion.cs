using UnityEngine;
using Cashing;
using Combat;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Explosion _explosion;
    
    private void Start()
    {
        transform.SetParent(null);
        
        _ownerEntity.Health.Died += Explode;
        
        _explosion.Initialize(_ownerEntity);
    }

    private async void Explode()
    {
        transform.position = _ownerEntity.transform.position;
        _explosion.transform.SetParent(null);
        
        _ownerEntity.Health.Died -= Explode;
        
        await _explosion.Explode();
    }
}