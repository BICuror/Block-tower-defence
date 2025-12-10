using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat;

public sealed class InfernoExplodeOnFullCharge : EntityModificator
{
    private WeaponPool<Explosion> _explosionPool;
    private ExplodeAndResetCharge _addedBehavior;
    private Explosion _explosionPrefab;
    
    public override void Enable()
    {
        Entity.StatContainer.AddStatIfDoesntExist<ExplosionDamage>(Args.GetArgument<float>("DefaultExplosionDamage"));
        Entity.StatContainer.AddStatIfDoesntExist<ExplosionRadius>(Args.GetArgument<float>("DefaultExplosionRadius"));
        
        _explosionPrefab = Args.GetArgument<GameObject>("ExplosionPrefab").GetComponent<Explosion>();
        _explosionPool = new WeaponPool<Explosion>(_explosionPrefab, 2, Entity);
        
        _addedBehavior = new ExplodeAndResetCharge();
        _addedBehavior.SetExplosionPool(_explosionPool);
        
        Entity.ComponentsContainer.Get<InfernoTower>().OnMaxChargeReached.AddBehaviour(_addedBehavior, BehaviourType.Additional);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<InfernoTower>().OnMaxChargeReached.RemoveAdditionalBehaviour(_addedBehavior);
        
        Entity.StatContainer.Remove<ExplosionDamage>();
        Entity.StatContainer.Remove<ExplosionRadius>();
        
        _explosionPool.DestroyPool();
    }

    private sealed class ExplodeAndResetCharge : CombatBehaviour
    {
        private WeaponPool<Explosion> _explosionPool;
        
        public void SetExplosionPool(WeaponPool<Explosion> explosionPool)
        {
            _explosionPool = explosionPool;
        }
        
        public override void Execute()
        {
            Explosion explosion = _explosionPool.GetPooledWeapon();

            explosion.transform.position = Entity.ComponentsContainer.Get<InfernoTower>().CurrentTarget.transform.position;
            explosion.Explode().Forget();
            
            Entity.ComponentsContainer.Get<InfernoTower>().ResetChargeAndTryFindTarget();
        }
    }
}