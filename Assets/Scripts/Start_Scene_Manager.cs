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
}
