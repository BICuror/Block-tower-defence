using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public sealed class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _restatButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private SettingsScreen _settingsScreen;
    
    private float _capturedTimeScale;

    private void Awake()
    {
        _continueButton.onClick.AddListener(QuitCurrentState);
        _settingsButton.onClick.AddListener(() => _settingsScreen.gameObject.SetActive(true));
        _restatButton.onClick.AddListener(Restart);
        _quitButton.onClick.AddListener(Quit);
    }

    public void QuitCurrentState()
    {
        if (gameObject.activeSelf)
        {
            if (_settingsScreen.gameObject.activeSelf) _settingsScreen.gameObject.SetActive(false);
            else gameObject.SetActive(false);
        }
        else gameObject.SetActive(true);
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    private void Quit() => Application.Quit();
    
    private void OnEnable()
    {
        Camera.main.GetComponent<GameController>().Disable();
        _capturedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }
    
    private void OnDisable()
    {
        Camera.main.GetComponent<GameController>().Enable();
        if (_capturedTimeScale == 0) _capturedTimeScale = 1f;
        Time.timeScale = _capturedTimeScale;
    }
}