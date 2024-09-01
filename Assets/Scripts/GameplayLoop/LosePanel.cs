using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class LosePanel : MonoBehaviour
{
    [SerializeField] private StatisticsManager _statisticsManager;
    [Inject] private WaveManager _waveManager;

    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private TextMeshProUGUI _wavesText;

    public void DisplayLosePanel()
    {
        DOVirtual.Float(0f, 1f, 3f, ChangeCanvasGroupAlpha);
    
        _wavesText.text = (_waveManager.GetCurrentWave() - 1).ToString();
        _killsText.text = _statisticsManager.Kills.ToString();
    }

    private void ChangeCanvasGroupAlpha(float value) => _canvasGroup.alpha = value;
}
