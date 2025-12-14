using Cashing;

namespace Combat.Animation
{
    public sealed class HitShaker : Shaker
    {
        [Cached] private EntityHealth _health;
        
        private void Start()
        {
            Initialize();
            _health.Damaged += Shake;
        }
    }
}

