using UnityEngine;

[CreateAssetMenu(fileName = "Unlock", menuName = "Unlocks/WaveUnlock")]

public sealed class WaveUnlock : Unlock
{
    [SerializeField] private int _requiredWaveReached;

    [SerializeField] private WorldGeneration.IslandData _islandData;

    [Header("RequirementText")]
    [SerializeField] private string _requirement;

    public override string GetRequirement() => _requirement;
    public override bool IsUnlocked() => _islandData.HasReachedWave(_requiredWaveReached);
}
