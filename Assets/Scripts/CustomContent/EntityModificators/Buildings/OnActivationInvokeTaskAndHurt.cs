using Cysharp.Threading.Tasks;

public sealed class OnActivationInvokeTaskAndHurt : EntityModificator
{
    private EntityCanvasAbilityIcon _abilityIcon;
    private float _activationDamage;
    
    public override void Enable()
    {
        _activationDamage = Args.GetArgument<float>("ActivationDamage");
        Entity.Activated += InvokeEntityTask;
        _abilityIcon = AddAbilityIcon(1);
        _abilityIcon.SetFillDuration(0.2f);
        UpdateAbilityIconCharge();
    }

    private void InvokeEntityTask()
    {
        TaskCycle taskCycle = Entity.ComponentsContainer.Get<TaskCycle>();

        if (taskCycle.IsPossibleToPerformTask() && CombatUtilities.TryTakeNonLethalDamage(Entity, _activationDamage))
        {
            taskCycle.PerformTask();
            UpdateAbilityIconCharge();
        }
    }

    private void UpdateAbilityIconCharge()
    {
        int maxCharges = (int)((Entity.Health.GetMaxHp() - _activationDamage) / _activationDamage);
        int charges = (int)((Entity.Health.GetHp() - _activationDamage) / _activationDamage);
        _abilityIcon.SetValue((float)charges / maxCharges).Forget();
    }
    
    public override void Disable()
    {
        Entity.Activated -= InvokeEntityTask;
        RemoveAbilityIcon(_abilityIcon);
    }
}