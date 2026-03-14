using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Explosion _explosion;
    
    private void Start()
    {
        _ownerEntity.Health.Died += Explode;
        
        _explosion.Initialize(_ownerEntity);
    }

    private void Explode()
    {
        _ownerEntity.Health.Died -= Explode;
        
        _explosion.Explode().Forget();
    }
}