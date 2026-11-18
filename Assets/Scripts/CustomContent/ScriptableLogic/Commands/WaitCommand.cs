using Cysharp.Threading.Tasks;
using Configs;
using ScriptableLogic.ArgumentItem;

namespace ScriptableLogic.Commands
{
    public sealed class WaitCommand : Command, ICommand
    {
        [ArgumentItem("WaitDuration")] private ValueArgumentItem _waitDuration;
        
        public async UniTask Execute()
        {
            if (Args.Owner) await UniTask.WaitForSeconds(_waitDuration.GetValue(), cancellationToken: Args.Owner.gameObject.GetCancellationTokenOnDestroy());
            else await UniTask.WaitForSeconds(_waitDuration.GetValue());
        }
    }
}