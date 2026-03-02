using UnityEngine;
using UnityEngine.UI;

public class Setting_Manager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public GameObject settingsMenu;

    private void Start()
    {
        // 슬라이더 이벤트 연결
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        SyncSliders();
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
    public void ToggleSettingsMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
            SyncSliders();
        }
    }
}
