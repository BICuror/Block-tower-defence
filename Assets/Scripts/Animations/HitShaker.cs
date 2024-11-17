using UnityEngine;

namespace Combat.Animation
{
    [RequireComponent(typeof(EntityHealth))]
    
    public sealed class HitShaker : Shaker
    {
        private void Awake()
        {
            CaptureDefaultScale();
    
            GetComponent<EntityHealth>().Damaged += Shake;
        }
    }
}

