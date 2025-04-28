using Combat;
using UnityEngine;
using Zenject;

public sealed class CrystalScheduler : MonoBehaviour
{
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
}