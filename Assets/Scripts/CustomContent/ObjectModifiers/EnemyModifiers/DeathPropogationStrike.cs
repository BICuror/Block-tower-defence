using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public class DeathPropogationStrike : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private PropogationStrike _propogationStrikePrefab;
    
    private void Start()
    {
        _ownerEntity.Health.Died += LaunchPropogationStrike;
    }

    private void LaunchPropogationStrike()
    {
        _ownerEntity.Health.Died -= LaunchPropogationStrike;
        
        PropogationStrike propogationStrike = Instantiate(_propogationStrikePrefab, transform.position, Quaternion.identity);
        propogationStrike.Initialize(_ownerEntity);
        
        StartPropogationStrike(propogationStrike).Forget();
    }

    private async UniTask StartPropogationStrike(PropogationStrike strike)
    {
        await strike.StartPropogationStrike(strike.transform.position);
        Destroy(strike.gameObject);
    }
}