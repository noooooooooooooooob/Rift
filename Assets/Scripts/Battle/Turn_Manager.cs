using UnityEngine;
using System.Collections;

public enum BattlePhase
{
    Deployment,
    Battle,
    Result
}

public class Turn_Manager : MonoBehaviour
{
    public BattlePhase currentPhase;
    public Deployment_Controller deploymentController;

    void Start()
    {
        currentPhase = BattlePhase.Deployment;
        deploymentController = GetComponent<Deployment_Controller>();
    }
    private IEnumerator State()
    {
        while (true)
        {
            switch (currentPhase)
            {
                case BattlePhase.Deployment:
                    yield return StartCoroutine(HandleDeploymentPhase());
                    break;
                case BattlePhase.Battle:
                    // yield return StartCoroutine(HandleBattlePhase());
                    break;
                case BattlePhase.Result:
                    // yield return StartCoroutine(HandleResultPhase());
                    break;
            }
        }
    }
    private IEnumerator HandleDeploymentPhase()
    {
        // 배치 단계 처리 로직
        Debug.Log("Deployment Phase Started");
        // 예: 플레이어가 유닛을 배치할 때까지 대기
        deploymentController.BeginDeployment();
        yield return new WaitUntil(() => deploymentController.IsDeploymentValid());
        Debug.Log("Deployment Phase Ended");
        currentPhase = BattlePhase.Battle;
    }
}
