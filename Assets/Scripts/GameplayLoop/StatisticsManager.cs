using UnityEngine;

public sealed class StatisticsManager : MonoBehaviour
{
    private int _currentKills;
    public int Kills => _currentKills;

    public void IncreaseKillsCount() => _currentKills++;
}
