using Cysharp.Threading.Tasks;

public abstract class GlobalEffect
{
    protected ArgumentsContainer Args;

    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    
    public abstract void Enable();
    public abstract void Disable();
}

namespace Combat.Effects
{
    public sealed class Effect
    {
        private readonly Domain _domain;

        public Effect(Domain domain)
        {
            _domain = domain;
        }
        
        public async UniTask Enable()
        {
            await _domain.Enable();
        }
        
        public async UniTask Disable()
        {
            await _domain.Disable();
        }
    }
}