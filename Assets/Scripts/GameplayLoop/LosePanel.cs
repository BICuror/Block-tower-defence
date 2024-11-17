using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Combat
{
    public class LosePanel : MonoBehaviour
    {
        [Inject] private WaveManager _waveManager;
    
        [SerializeField] private CanvasGroup _canvasGroup;
    
        [SerializeField] private TextMeshProUGUI _killsText;
        [SerializeField] private TextMeshProUGUI _wavesText;
    
        public void DisplayLosePanel()
        {
            DOVirtual.Float(0f, 1f, 3f, ChangeCanvasGroupAlpha);
        
            _wavesText.text = (_waveManager.GetCurrentWave() - 1).ToString();
        }
    
        private void ChangeCanvasGroupAlpha(float value) => _canvasGroup.alpha = value;
    }
}

