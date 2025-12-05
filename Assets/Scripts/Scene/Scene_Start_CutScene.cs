using UnityEngine;
using UnityEngine.Playables;

public class CutSceneCallback : MonoBehaviour
{
    public Turn_Manager turnManager;
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        if (turnManager != null)
        {
            turnManager.OnCutSceneEnd();
        }
    }
}