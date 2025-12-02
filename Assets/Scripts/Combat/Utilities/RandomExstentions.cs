using UnityEngine;

public static class RandomExstentions
{
    public static void ReInitializeUnityRandom()
    {
        Random.InitState(new System.Random().Next(int.MinValue, int.MaxValue));
    }
}
