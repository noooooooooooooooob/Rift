using UnityEngine;
using UnityEngine.Playables;

public class Battle_Start_CutScene : MonoBehaviour
{
    public PlayableDirector director;
    public Transform attacker;
    public Transform[] target;
    public void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }
    void Start()
    {
        director.Play();
        StartBattleCutscene(attacker, target);
    }
    public void StartBattleCutscene(Transform attacker, Transform[] target)
    {
        this.attacker = attacker;
        this.target = target;
        // Focus Layer 지정
        Method.SetLayerRecursively(attacker.gameObject, LayerMask.NameToLayer("Focus"));
        foreach (var t in target)
        {
            Method.SetLayerRecursively(t.gameObject, LayerMask.NameToLayer("Focus"));
        }
        director.stopped += EndCutscene;
    }
    public void EndCutscene(PlayableDirector d)
    {
        director.stopped -= EndCutscene;
        foreach (var obj in target)
        {
            Method.SetLayerRecursively(obj.gameObject, LayerMask.NameToLayer("Default"));
        }
        if (attacker != null)
        {
            Method.SetLayerRecursively(attacker.gameObject, LayerMask.NameToLayer("Default"));
        }
    }
}
