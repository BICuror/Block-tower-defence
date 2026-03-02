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
    private PropogationDamageMiultiplier _propogationDamageMultiplier;
    private PropogationRadius _propogationRadius;
    private Damage _damage;
    private float _lastDamage;

    protected override void OnInitialized()
    {
        _propogationDamageMultiplier = OwnerEntity.StatContainer.Get<PropogationDamageMiultiplier>();
        _propogationRadius = OwnerEntity.StatContainer.Get<PropogationRadius>();
        _damage = OwnerEntity.StatContainer.Get<Damage>();
    }
    
    public async UniTask StartPropogationStrike(CombatEntity initialEntity, Transform initialTransform = null)
    {
        _targetedEnemes.Clear();
        
        if (initialTransform)
        {   
            _beamSystem.SetSource(initialTransform);
            _beamSystem.ReachTargetAndSetIt(initialEntity.transform, initialTransform.position, _stepDuration).Forget();
        }
        else
        {
            _beamSystem.DisableBeam();
            initialTransform = initialEntity.transform;
        }
        
        await UniTask.WaitForSeconds(_stepDuration);

        CombatEntity currentEntity = initialEntity;
        
        _lastDamage = _damage.Value;
        
        while (currentEntity)
        {
            currentEntity.Health.ReceiveEnemyDamage(_lastDamage, OwnerEntity);
            _lastDamage *= _propogationDamageMultiplier.Value;
            _targetedEnemes.Add(currentEntity);
            
            await UniTask.WaitForSeconds(_stepDuration);
            
            if (!currentEntity) break;
            
            CombatEntity newTargetEntity = GetEnemiesInRadius(currentEntity.transform.position);
            
            if (!newTargetEntity) break;
            
            if (Vector3.Distance(newTargetEntity.transform.position, currentEntity.transform.position) > _propogationRadius.Value * 1.5f) break;

            MoveBeamToNewPosition(initialTransform.position, currentEntity.transform, newTargetEntity.transform, _stepDuration).Forget();
            
            initialTransform = currentEntity.transform;
            
            currentEntity = newTargetEntity;
        }

        gameObject.SetActive(false);
        _beamSystem.DisableBeam();
    }

    private async UniTask MoveBeamToNewPosition(Vector3 previousPosition, Transform currentTransform, Transform desiredTransform, float duration)
    {
        _beamSystem.SetSource(currentTransform.transform);
        await _beamSystem.ReachTargetAndSetIt(currentTransform.transform, previousPosition, _stepDuration / 2f);
        await _beamSystem.ReachTargetAndSetIt(desiredTransform, currentTransform .transform.position, _stepDuration / 2f);
    }

    private CombatEntity GetEnemiesInRadius(Vector3 centerPosition)
    {
        Collider[] enemyColliders = Physics.OverlapSphere(centerPosition, _propogationRadius.Value, _layerSetting.GetLayerMask());

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i].gameObject.TryGetComponent(out CombatEntity entity))
            {
                if (!_targetedEnemes.Contains(entity))
                {
                    return entity;
                }
            }
        }

        return null;
    }
}