using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using GameControls;
using DG.Tweening;
using UnityEngine;
using Zenject;

public sealed class GameEndScreen : MonoBehaviour
{
    [Inject] private TimeController _timeController;
    [Inject] private GameController _gameController;

    [Header("UI Elements")] 
    [SerializeField] private UIElementKeyboardController _pauseController;
    [SerializeField] private List<CanvasGroup> _canvasGroups;
    
    [SerializeField] private Button _restatButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _restatButton.onClick.AddListener(Restart);
        _quitButton.onClick.AddListener(Quit);
    }
    
    private void Restart()
    {
        CleanUp();
        SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit()
    {
        CleanUp();
        SceneManager.LoadScene("Menu");
    }
    
    private void CleanUp()
    {
        _gameController.Dispose();
    }
    
    public async UniTask Enable()
    {
        _pauseController.Quit.RemoveAllListeners();
        _timeController.Pause();
        
        gameObject.SetActive(true);
        
        _canvasGroups.ForEach(group => group.alpha = 0f);
        
        for (int i = 0; i < _canvasGroups.Count; i++)
        {
            _canvasGroups[i].DOFade(1f, 0.5f).SetEase(Ease.InCubic).SetLink(gameObject).From(0f).SetUpdate(true);
        }
    }
}