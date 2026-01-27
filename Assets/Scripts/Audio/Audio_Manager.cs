using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//사운드의 타입이다. 사운드를 중단을 식별하기위해 사용한다.
public enum SoundType
{
    BGM,
    SFX,
}

public class Audio_Manager : MonoBehaviour
{
    // 오디오 믹서
    [SerializeField] private AudioMixer audioMixer;

    // 옵션에서 설정된 현재 배경음악과 효과 사운드의 불륨
    private float currentMasterVolume, currentBGMVolume, currentEffectVolume;

    // 클립들을 담는 딕셔너리
    private Dictionary<string, AudioClip> clipsDictionary;

    // 사전에 미리 로드하여 사용할 클립들
    [SerializeField] private AudioClip[] preloadClips;

    // Loop로 재생되는 오디오를 추후에 접근하여 제거
    private List<TemporarySoundPlayer> instantiatedSounds;

    // 슬라이더를 통해 불륨을 조절하기 위한 UI 요소
    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    private static Audio_Manager instance;
    public static Audio_Manager Instance
    {
        get
        {
            if (instance == null) instance = new Audio_Manager();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        clipsDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in preloadClips)
        {
            clipsDictionary.Add(clip.name, clip);
        }

        instantiatedSounds = new List<TemporarySoundPlayer>();

        // 슬라이더의 초기값을 설정
        if (masterSlider != null)
        {
            masterSlider.value = Mathf.Pow(10, currentMasterVolume / 20);
            masterSlider.onValueChanged.AddListener(sliderMasterValueChanged);
        }
        if (bgmSlider != null)
        {
            bgmSlider.value = Mathf.Pow(10, currentBGMVolume / 20);
            bgmSlider.onValueChanged.AddListener(sliderBGMValueChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = Mathf.Pow(10, currentEffectVolume / 20);
            sfxSlider.onValueChanged.AddListener(sliderSFXValueChanged);
        }
    }

    // 오디오의 이름을 기반으로 찾는다.
    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = clipsDictionary[clipName];

        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다."); }

        return clip;
    }

    // 사운드를 재생할 때, 루프 형태로 재생된경우에는 나중에 제거하기위해 리스트에 저장
    private void AddToList(TemporarySoundPlayer soundPlayer)
    {
        instantiatedSounds.Add(soundPlayer);
    }

    // 루프 사운드 중 리스트에 있는 오브젝트를 이름으로 찾아 제거
    public void StopLoopSound(string clipName)
    {
        foreach (TemporarySoundPlayer audioPlayer in instantiatedSounds)
        {
            if (audioPlayer.ClipName == clipName)
            {
                instantiatedSounds.Remove(audioPlayer);
                Destroy(audioPlayer.gameObject);
                return;
            }
        }

        Debug.LogWarning(clipName + "을 찾을 수 없습니다.");
    }

    public void PlaySound(string clipName, float delay = 0f, bool isLoop = false, SoundType type = SoundType.SFX)
    {
        GameObject obj = new GameObject("TemporarySoundPlayer");
        TemporarySoundPlayer soundPlayer = obj.AddComponent<TemporarySoundPlayer>();

        //루프를 사용하는경우 사운드를 저장한다.
        if (isLoop) { AddToList(soundPlayer); }

        soundPlayer.InitSound(GetClip(clipName));
        soundPlayer.Play(audioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    //옵션을 변경할 때 소리의 불륨을 조절하는 함수
    public void SetVolume(SoundType type, float value)
    {
        audioMixer.SetFloat(type.ToString(), value);
    }

    public void sliderMasterValueChanged(float value)
    {
        float result = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat("Master", result);
        currentMasterVolume = result;
    }
    public void sliderBGMValueChanged(float value)
    {
        float result = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        SetVolume(SoundType.BGM, result);
        currentBGMVolume = result;
    }
    public void sliderSFXValueChanged(float value)
    {
        float result = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        SetVolume(SoundType.SFX, result);
        currentEffectVolume = result;
    }
}