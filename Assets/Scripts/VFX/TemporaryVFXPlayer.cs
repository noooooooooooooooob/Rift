using System.Collections;
using UnityEngine;

/// <summary>
/// VFX 재생 완료 후 자동으로 풀에 반환하는 컴포넌트
/// </summary>
public class TemporaryVFXPlayer : MonoBehaviour
{
    private VFX_Manager manager;
    private string effectName;
    private ParticleSystem[] particleSystems;
    private float duration;

    private Coroutine returnCoroutine;

    public void Initialize(VFX_Manager manager, string effectName)
    {
        this.manager = manager;
        this.effectName = effectName;

        // 파티클 시스템 캐싱 및 최대 지속 시간 계산
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        CalculateDuration();
    }

    private void CalculateDuration()
    {
        duration = 0f;

        foreach (var ps in particleSystems)
        {
            var main = ps.main;

            // 루프가 아닌 파티클만 계산
            if (!main.loop)
            {
                float psDuration = main.duration + main.startLifetime.constantMax;
                if (psDuration > duration)
                {
                    duration = psDuration;
                }
            }
        }

        // 최소 지속 시간 보장
        if (duration <= 0f)
        {
            duration = 1f;
        }
    }

    public void Play()
    {
        // 모든 파티클 재생
        foreach (var ps in particleSystems)
        {
            ps.Clear();
            ps.Play();
        }

        // 자동 반환 코루틴 시작
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnAfterDuration());
    }

    public void Stop()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        manager.ReturnToPool(effectName, gameObject);
    }

    private IEnumerator ReturnAfterDuration()
    {
        yield return new WaitForSeconds(duration);

        manager.ReturnToPool(effectName, gameObject);
        returnCoroutine = null;
    }
}
