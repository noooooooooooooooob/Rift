using UnityEngine;

[ExecuteAlways]
public class RadialFadeController : MonoBehaviour
{
    public Material material;  // RadialFade.mat
    [Range(0f, 1.7f)]
    public float radius = 0f;
    public float softness = 0.05f;
    public Vector2 center = new Vector2(0.5f, 0.5f);

    void OnValidate() { Apply(); }
    void Update()     { Apply(); }  // Timeline/Animator가 radius만 바꿔도 매 프레임 반영

    void Apply()
    {
        if (!material) return;
        material.SetFloat("_Radius",   radius);
        material.SetFloat("_Softness", softness);
        material.SetVector("_Center",  new Vector4(center.x, center.y, 0, 0));
    }
}
