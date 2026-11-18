using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;
using UnityEngine.Serialization;

public class DeathPropogationStrike : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [FormerlySerializedAs("_propogationStrikePrefab")] [SerializeField] private RicochetStrike _ricochetStrikePrefab;
    
    private void Start()
    {
        _ownerEntity.Health.Died += LaunchPropogationStrike;
    }

    private void LaunchPropogationStrike()
    {
        _ownerEntity.Health.Died -= LaunchPropogationStrike;
        
        RicochetStrike ricochetStrike = Instantiate(_ricochetStrikePrefab, transform.position, Quaternion.identity);
        ricochetStrike.Initialize(_ownerEntity);
        
        StartPropogationStrike(ricochetStrike).Forget();
    }

    private async UniTask StartPropogationStrike(RicochetStrike strike)
    {
        await strike.StartPropogationStrike(strike.transform.position);
        Destroy(strike.gameObject);
    }
}