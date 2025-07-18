using UnityEngine;

[CreateAssetMenu(fileName = "GlobalStatInitializerConfig", menuName = "Generation/GlobalStatInitializerConfig")]

public sealed class GlobalStatInitializerConfig : ScriptableObject
{
    [SerializeField] private StatInitializer[] _statInitializers;
    
    public StatInitializer[] StatInitializers => _statInitializers;
    
    private void OnValidate()
    {
        for (int i = 0; i < _statInitializers.Length; i++)
        {
            StatInitializer _statInitializer = _statInitializers[i];
            
            _statInitializer.StructName = _statInitializer.StatData.GetStatType().ToString();
        }
    }
}