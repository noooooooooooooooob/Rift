using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 전투 시작 컷신 관리 클래스
/// Timeline 기반 컷신을 재생하고 캐릭터 레이어를 Focus로 전환하여 선택적 렌더링
/// </summary>
public class Battle_Start_CutScene : MonoBehaviour
{
    public PlayableDirector director;  // Timeline 재생 컨트롤러
    public Transform attacker;  // 공격자 Transform
    public Transform[] target;  // 대상(들) Transform 배열

    /// <summary>
    /// 초기화: PlayableDirector 컴포넌트 가져오기
    /// </summary>
    public void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    /// <summary>
    /// 시작 시 컷신 재생
    /// </summary>
    void Start()
    {
        director.Play();
        StartBattleCutscene(attacker, target);
    }

    /// <summary>
    /// 전투 컷신 시작
    /// 공격자와 대상들을 Focus 레이어로 전환하여 컷신 카메라가 렌더링하도록 설정
    /// </summary>
    public void StartBattleCutscene(Transform attacker, Transform[] target)
    {
        this.attacker = attacker;
        this.target = target;
        // Focus Layer 지정 (컷신 카메라가 이 레이어만 렌더링)
        Method.SetLayerRecursively(attacker.gameObject, LayerMask.NameToLayer("Focus"));
        foreach (var t in target)
        {
            Method.SetLayerRecursively(t.gameObject, LayerMask.NameToLayer("Focus"));
        }
        director.stopped += EndCutscene;  // 컷신 종료 시 콜백 등록
    }

    /// <summary>
    /// 컷신 종료 콜백
    /// 모든 캐릭터를 Default 레이어로 복구
    /// </summary>
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
