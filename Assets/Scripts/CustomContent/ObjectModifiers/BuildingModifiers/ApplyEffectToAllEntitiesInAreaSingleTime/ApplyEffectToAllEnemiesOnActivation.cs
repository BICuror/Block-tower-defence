using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class ApplyEffectToAllEnemiesOnActivation : ApplyEffectOnceToEntitiesInArea
{
    [Cached] private EntityCanvas _canvas;
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Sprite _iconSprite;
    private EntityCanvasAbilityIcon _abilityIcon;
    
    private void Start()
    {
        base.Start();
        _ownerEntity.Activated += ApplyEffect;
        _abilityIcon = _canvas.AddAbilityIcon(_iconSprite, (float)CurrentCharges / MaxCharges);
    }
    
    private void OnDestroy()
    {
        base.OnDestroy();
        
        _ownerEntity.Activated -= ApplyEffect;
    }

    protected override void OnChargesValueChanged() => UpdateIconState();
    
    private void UpdateIconState()
    {
        _abilityIcon.SetValue((float)CurrentCharges / MaxCharges).Forget();
    }
}