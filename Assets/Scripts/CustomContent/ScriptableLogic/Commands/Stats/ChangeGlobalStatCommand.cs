using Cysharp.Threading.Tasks;
using Combat.Stats;
using Zenject;
using Configs;

namespace ScriptableLogic.Commands
{
    public sealed class ChangeGlobalStatCommand : StatCommand
    {
        [Inject] private GlobalStatContainer _globalStatContainer;

        [Argument("FlatChange")] private float _flatChange;
        [Argument("MultiplierChange")] private float _multiplierChange;

        private StatModifier _statModifier;

        public override UniTask Enable()
        {
            _statModifier = new StatModifier(_flatChange, _multiplierChange);

            _globalStatContainer.Get(StatConfig.GetStatType()).AddStatModifier(_statModifier);

            return UniTask.CompletedTask;
        }

        public override UniTask Disable()
        {
            _globalStatContainer.Get(StatConfig.GetStatType()).RemoveStatModifier(_statModifier);

            return UniTask.CompletedTask;
        }
    }
}