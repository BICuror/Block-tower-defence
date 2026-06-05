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
        float charge = Entity.Health.GetHp() / Entity.Health.GetMaxHp();
        _abilityIcon.SetValue(charge).Forget();
    }
    
    public override void Disable()
    {
        Entity.Activated -= InvokeEntityTask;
        RemoveAbilityIcon(_abilityIcon);
    }
}