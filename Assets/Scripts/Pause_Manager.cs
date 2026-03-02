using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause_Manager : MonoBehaviour
{
    public static Pause_Manager instance;

    [Header("UI")]
    public GameObject pausePanel;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button returnButton;

    public bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            returnButton.onClick.AddListener(() =>
            {
                Resume();
                SceneManager.LoadScene("Start Scene");
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start Scene")
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // 슬라이더 이벤트 연결
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnMasterSliderChanged(float value)
    {
        Audio_Manager.Instance.sliderMasterValueChanged(value);
    }

    private void OnBGMSliderChanged(float value)
    {
        Audio_Manager.Instance.sliderBGMValueChanged(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        Audio_Manager.Instance.sliderSFXValueChanged(value);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        SyncSliders();
    }

    /// <summary>
    /// Audio_Manager의 현재 볼륨 값과 슬라이더 동기화
    /// </summary>
    private void SyncSliders()
    {
        if (Audio_Manager.Instance == null) return;

        if (masterSlider != null)
            masterSlider.SetValueWithoutNotify(Audio_Manager.Instance.GetMasterSliderValue());
        if (bgmSlider != null)
            bgmSlider.SetValueWithoutNotify(Audio_Manager.Instance.GetBGMSliderValue());
        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(Audio_Manager.Instance.GetSFXSliderValue());
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}
