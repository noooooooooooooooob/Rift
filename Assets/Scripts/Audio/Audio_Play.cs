using UnityEngine;

public class Audio_Play : MonoBehaviour
{
    public void PlaySound(string soundName, bool loop = false, SoundType soundType = SoundType.BGM)
    {
        Audio_Manager.Instance.PlaySound(soundName, 0, loop, soundType);
    }
}
