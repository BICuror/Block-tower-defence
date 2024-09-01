using UnityEngine;

public sealed class WaveCrystal : Crystal
{
    public override void Activate() 
    {
        StopFromDestroying();

        CrystalUsed.Invoke();
    }
}
