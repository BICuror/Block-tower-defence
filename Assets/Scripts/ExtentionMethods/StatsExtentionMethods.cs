using UnityEngine;
using System;

public static class StatsExtentionMethods
{
    public static StatContainer GetStatContainer(this MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour.TryGetComponent<StatContainer>(out StatContainer container))
        {
            return container;
        }
        return null;
    }

    public static T GetStat<T>(this MonoBehaviour monoBehaviour) where T : Stat
    {
        if (monoBehaviour.TryGetComponent<StatContainer>(out StatContainer container))
        {
            return container.GetStat<T>();
        }
        return null;
    }
}