using UnityEngine;
using WorldGeneration;
using TMPro;

public sealed class LevelSelector : MonoBehaviour
{
    [Header("IslandDataSettings")]
    [SerializeField] private IslandDataContainer _islandDataContainer;
    [SerializeField] private IslandData[] _islandDatas;    
    [SerializeField] private Unlock[] _unlocks;

    [Header("InfoDisplayers")]
    [SerializeField] private TextMeshProUGUI _locationNameText;

    [Header("UnlockDisplay")]
    [SerializeField] private GameObject _lockedDisplay;
    [SerializeField] private TextMeshProUGUI _requirementText;

    [Header("Generation")]
    [SerializeField] private IslandGenerator _islandGeneration;
    [SerializeField] private MaterialSetter _materialSetter;


    private void OnEnable()
    {
        DisplayIslandData(GetCurrentIslandDataIndex());
    }

    private void DisplayIslandData(int index)
    {
        _islandDataContainer.SetData(_islandDatas[index]);

        _islandGeneration.GenerateIsland();
        _materialSetter.UpdateMaterialsTextures();

        _locationNameText.text = _islandDatas[index].IslandName;

        if (_unlocks[index].IsUnlocked())
        {
            _lockedDisplay.SetActive(false);
        }
        else 
        {
            _lockedDisplay.SetActive(true);
            _requirementText.text = _unlocks[index].GetRequirement();
        }
    }


    public void NextIslandData() 
    {
        int index = GetCurrentIslandDataIndex() + 1;

        if (index >= _islandDatas.Length) index = 0;

        DisplayIslandData(index);
    }

    public void PreviousIslandData() 
    {
        int index = GetCurrentIslandDataIndex() - 1;

        if (index < 0) index = _islandDatas.Length - 1;

        DisplayIslandData(index);
    }

    private int GetCurrentIslandDataIndex()
    {
        for (int i = 0; i < _islandDatas.Length; i++)
        {
            if (_islandDataContainer.Data == _islandDatas[i]) return i;
        }

        return 0;
    }
}
