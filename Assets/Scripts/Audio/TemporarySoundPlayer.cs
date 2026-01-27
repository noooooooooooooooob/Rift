using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class TemporarySoundPlayer : MonoBehaviour
{
    private AudioSource mAudioSource;
    public string ClipName
    {
        get
        {
            return mAudioSource.clip.name;
        }
    }

    public void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioMixerGroup audioMixer, float delay, bool isLoop)
    {
        mAudioSource.outputAudioMixerGroup = audioMixer;
        mAudioSource.loop = isLoop;
        mAudioSource.Play();

        if (!isLoop) { StartCoroutine(COR_DestroyWhenFinish(mAudioSource.clip.length)); }
    }

    public void InitSound(AudioClip clip)
    {
        mAudioSource.clip = clip;
    }
    private IEnumerator COR_DestroyWhenFinish(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        Destroy(gameObject);
    }
}