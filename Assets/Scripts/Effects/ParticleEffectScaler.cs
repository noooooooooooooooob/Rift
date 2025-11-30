using UnityEngine;

public class ParticleEffectScaler : MonoBehaviour
{
    [Min(0.01f)]
    public float scale = 1f;   // 인스펙터에서 조절할 스케일

    // 이전 스케일 값 (에디터에서도 유지되도록 직렬화)
    [SerializeField, HideInInspector]
    private float lastScale = 1f;

    // 인스펙터 값이 바뀔 때마다 자동 호출 (에디터 전용 느낌)
    private void OnValidate()
    {
        // 플레이 중엔 아무 것도 하지 않음
        if (Application.isPlaying)
            return;

        ApplyScale_EditorOnly();
    }

    // 에디터에서만 쓰는 내부 함수
    private void ApplyScale_EditorOnly()
    {
        if (scale <= 0f)
            return;

        float factor = scale / lastScale;
        lastScale = scale;

        var systems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (var ps in systems)
        {
            // Transform 스케일
            ps.transform.localScale *= factor;

            // Main 모듈
            var main = ps.main;
            main.startSizeMultiplier     *= factor;
            main.startSpeedMultiplier    *= factor;
            main.startLifetimeMultiplier *= factor;

            // Shape 모듈
            var shape = ps.shape;
            shape.radius *= factor;
        }
    }

    // 처음 붙였을 때 기준값 맞춰주고 싶으면 선택
    private void Reset()
    {
        lastScale = scale;
    }
}
