using UnityEngine;

public class SpriteBaker : MonoBehaviour
{
    public Camera cam;
    public RenderTexture rt;
    public string fileName = "BakedChar.png";

    public Transform targetCharacterRoot;  // 선택한 캐릭터 루트
    public string characterTag = "Character"; // 모든 캐릭터에 공통 태그

    [ContextMenu("Bake Selected")]
    public void BakeSelected()
    {
        if (cam == null || rt == null || targetCharacterRoot == null)
        {
            Debug.LogError("세팅이 비어 있음");
            return;
        }

        // 1) 다른 캐릭터들 렌더 끄기
        var allCharacters = GameObject.FindGameObjectsWithTag(characterTag);
        var disabledRenderers = new System.Collections.Generic.List<Renderer>();

        foreach (var ch in allCharacters)
        {
            if (ch.transform == targetCharacterRoot) continue;

            foreach (var r in ch.GetComponentsInChildren<Renderer>())
            {
                if (r.enabled)
                {
                    r.enabled = false;
                    disabledRenderers.Add(r);
                }
            }
        }

        // 2) 캡처
        var prevTarget = cam.targetTexture;
        var prevActive = RenderTexture.active;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = prevActive;
        cam.targetTexture = prevTarget;

        // 3) 꺼놨던 렌더 다시 켜기
        foreach (var r in disabledRenderers)
            r.enabled = true;

        // 4) 저장
        var path = System.IO.Path.Combine(Application.dataPath, fileName);
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("Saved: " + path);

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}
