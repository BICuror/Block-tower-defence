using UnityEngine;

namespace Combat.Animation
{
    [RequireComponent(typeof(EntityHealth))]
    
    public sealed class HitShaker : Shaker
    {
        private void Awake()
        {
            GetComponent<EntityHealth>().Damaged += Shake;
        }
    }
}

