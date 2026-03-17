using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat;

public sealed class PropogationStrike : WeaponBase
{
    [SerializeField] private LayerSetting _layerSetting;
    [SerializeField] private BeamSystem _beamSystem;
    [SerializeField] private float _stepDuration;
    private List<CombatEntity> _targetedEnemes = new();
    private PropogationDamageMultiplier _propogationDamageMultiplier;
    private PropogationRadius _propogationRadius;
    private Damage _damage;
    private float _lastDamage;

    protected override void OnInitialized()
    {
        _propogationDamageMultiplier = OwnerEntity.StatContainer.Get<PropogationDamageMultiplier>();
        _propogationRadius = OwnerEntity.StatContainer.Get<PropogationRadius>();
        _damage = OwnerEntity.StatContainer.Get<Damage>();
    }

    public async UniTask StartPropogationStrike(Vector3 startPosition)
    {
        if (TryGetEntityInRadius(startPosition, out CombatEntity currentEntity))
        {
            await StartPropogationStrike(currentEntity, startPosition);
        }
    }
    
    public async UniTask StartPropogationStrike(CombatEntity initialEntity, Vector3 startPosition)
    {
        await _beamSystem.ReachTargetAndSetIt(startPosition, initialEntity.transform, _stepDuration);
            
        await StartPropogationStrike(initialEntity);
    }
    
    private async UniTask StartPropogationStrike(CombatEntity currentEntity)
    {
        _beamSystem.DisableBeam();
        _targetedEnemes.Clear();
        
        _lastDamage = _damage.Value;
    
        while (currentEntity && currentEntity.gameObject.activeSelf)
        {
            Vector3 hitPosition = currentEntity.transform.position;
            
            _targetedEnemes.Add(currentEntity);
            currentEntity.Health.ReceiveEnemyDamage(_lastDamage, OwnerEntity); 
            _lastDamage *= _propogationDamageMultiplier.Value; 
            
            if (TryGetEntityInRadius(hitPosition, out CombatEntity newTargetEntity))
            {
                await MoveBeamToNewPosition(hitPosition, newTargetEntity.transform);
                
                currentEntity = newTargetEntity;
            }
            else break;
        }
        
        gameObject.SetActive(false);
        _beamSystem.DisableBeam();
    }

    private async UniTask MoveBeamToNewPosition(Vector3 startPositon, Transform desiredTransform)
    {
        float distance = Vector3.Distance(startPositon, desiredTransform.position);

        if (distance > 3) distance *= 0.3f;
        
        await _beamSystem.ReachTargetAndSetIt(startPositon, desiredTransform, _stepDuration * distance);
    }

    private bool TryGetEntityInRadius(Vector3 centerPosition, out CombatEntity entity)
    {
        entity = null;
        
        Collider[] enemyColliders = Physics.OverlapSphere(centerPosition, _propogationRadius.Value, _layerSetting.GetLayerMask());

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i].gameObject.TryGetComponent(out entity))
            {
                if (!_targetedEnemes.Contains(entity))
                {
                    return true;
                }
            }
        }

        return false;
    }
}