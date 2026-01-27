using UnityEngine;
using UnityEngine.UI;

public class Setting_Manager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

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

        if (masterSlider != null && Audio_Manager.Instance.masterSlider != null)
            masterSlider.value = Audio_Manager.Instance.masterSlider.value;
        if (bgmSlider != null && Audio_Manager.Instance.bgmSlider != null)
            bgmSlider.value = Audio_Manager.Instance.bgmSlider.value;
        if (sfxSlider != null && Audio_Manager.Instance.sfxSlider != null)
            sfxSlider.value = Audio_Manager.Instance.sfxSlider.value;
    }
    public void ToggleSettingsMenu()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(!gameObject.activeSelf);
            SyncSliders();
        }
    }
}
