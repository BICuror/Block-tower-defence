using Cysharp.Threading.Tasks;
using Configs;
using Combat;

public sealed class DealNonLethalDamageCommand : Command, ICommand<CombatEntity>
{
    [Argument("DamageAmount")] private float _damageAmount;

    public UniTask Execute(CombatEntity args)
    {        
        CombatUtilities.TryTakeNonLethalDamage(args, _damageAmount);
        
        return UniTask.CompletedTask;
    }
}

public sealed class DealNonLethalDamageToSelfCommand : Command, ICommand
{
    [Argument("DamageAmount")] private float _damageAmount;

    public UniTask Execute()
    {
        CombatUtilities.TryTakeNonLethalDamage(Args.Owner, _damageAmount);
        
        return UniTask.CompletedTask;
    }
}