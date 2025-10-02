using DG.Tweening;
using UnityEngine;

public sealed class HeavyMortarCore : EntityModificator
{
    public override void Enable()
    {
        ScaleCore scaleCoreBehaviour = new ScaleCore(Args.GetArgument<float>("InitialScale"), Args.GetArgument<float>("FinalScale"), Entity.StatContainer.Get<TravelTime>());
        
        Entity.ComponentsContainer.Get<MortarTower>().CoreLaunched.AddBehaviour(scaleCoreBehaviour, BehaviourType.Override);
        Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.AddBehaviour(new IgnoreCoreLanding(), BehaviourType.Override);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<MortarTower>().CoreLaunched.RemoveOverrideBehaviour();
        Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.RemoveOverrideBehaviour();
    }

    private sealed class IgnoreCoreLanding : CombatBehaviour<Vector3>
    {
        public override void Execute(Vector3 dynamicArg) {}
    }
    
    private sealed class ScaleCore : CombatBehaviour<Transform>
    {
        private float _initialScale;
        private float _finalScale;
        private TravelTime _travelTime;
        
        public ScaleCore(float initialScale, float finalScale, TravelTime travelTime)
        {
            _initialScale = initialScale;
            _finalScale = finalScale;
            _travelTime = travelTime;
        }
        
        public override void Execute(Transform core)
        {
            core.DOScale(_finalScale, _travelTime.Value).From(_initialScale).SetEase(Ease.Linear);
        }
    }
}
