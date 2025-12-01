using Cashing;

namespace Combat.Animation
{
    public sealed class HitShaker : Shaker
    {
        [Cached] private EntityHealth _health;
        
        private void Start()
        {
            _health.Damaged += Shake;
        }
    }
}

