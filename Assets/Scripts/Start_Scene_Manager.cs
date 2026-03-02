using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Start_Scene_Manager : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    private void Start()
    {
        
        Audio_Manager.Instance.PlaySound("Start Scene BGM",0,true,SoundType.BGM);
    }
}
