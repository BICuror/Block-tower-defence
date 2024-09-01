using UnityEngine;
using WorldGeneration;

public sealed class MenuEntryPoint : MonoBehaviour
{
    [SerializeField] private IslandGenerator _islandGenerator;

    private void Start() => _islandGenerator.GenerateIsland();
}
