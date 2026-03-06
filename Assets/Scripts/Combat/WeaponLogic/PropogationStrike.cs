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
    
    public async UniTask StartPropogationStrike(CombatEntity initialEntity, Transform sourceTransform = null)
    {
        _targetedEnemes.Clear();
        
        if (sourceTransform)
        {   
            _beamSystem.SetSource(sourceTransform);
            await _beamSystem.ReachTargetAndSetIt(initialEntity.transform, sourceTransform.position, _stepDuration);
        }
        else
        {
            _beamSystem.DisableBeam();
            sourceTransform = initialEntity.transform;
        }

        CombatEntity currentEntity = initialEntity;
        
        _lastDamage = _damage.Value;
        
        while (currentEntity && currentEntity.gameObject.activeSelf)
        {
            currentEntity.Health.ReceiveEnemyDamage(_lastDamage, OwnerEntity);
            _lastDamage *= _propogationDamageMultiplier.Value;
            _targetedEnemes.Add(currentEntity);

            if (TryGetEntityInRadius(currentEntity.transform.position, out CombatEntity newTargetEntity))
            {
                await MoveBeamToNewPosition(sourceTransform.position, currentEntity.transform, newTargetEntity.transform);
                
                sourceTransform = currentEntity.transform;
                
                currentEntity = newTargetEntity;
            }
            else break;
        }

        gameObject.SetActive(false);
        _beamSystem.DisableBeam();
    }

    private async UniTask MoveBeamToNewPosition(Vector3 previousPosition, Transform currentTransform, Transform desiredTransform)
    {
        _beamSystem.SetSource(currentTransform.transform);
        
        float distance = Vector3.Distance(previousPosition, desiredTransform.position);

        if (distance > 3) distance *= 0.3f;
        
        await _beamSystem.ReachTargetAndSetIt(currentTransform.transform, previousPosition, _stepDuration / 2f * distance);
        if (!desiredTransform) return;
        await _beamSystem.ReachTargetAndSetIt(desiredTransform, currentTransform.transform.position, _stepDuration / 2f * distance);
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