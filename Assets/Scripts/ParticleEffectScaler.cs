using UnityEngine;

[ExecuteAlways] // 에디터에서도 바로 반영되게
public class ParticleEffectScaler : MonoBehaviour
{
    [Min(0.01f)]
    public float scale = 1f;     // 원하는 전체 스케일
    private float lastScale = 1f;

    [ContextMenu("Apply Scale")]
    public void ApplyScale()
    {
        if (scale <= 0f) return;

        float factor = scale / lastScale;
        lastScale = scale;

        // 자식 포함 모든 파티클 가져오기
        var systems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (var ps in systems)
        {
            // Transform 스케일
            ps.transform.localScale *= factor;

            // Main 모듈
            var main = ps.main;
            main.startSizeMultiplier      *= factor;
            main.startSpeedMultiplier     *= factor;
            main.startLifetimeMultiplier  *= factor;

            // Shape 모듈
            var shape = ps.shape;
            shape.radius *= factor;

            // 필요하면 다른 모듈도 추가로 스케일 (예: Velocity over Lifetime 등)
        }
    }

    // 에디터에서 값 바꿀 때 자동 적용하고 싶으면
    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            ApplyScale();
        }
    }
}
