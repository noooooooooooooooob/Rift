using UnityEngine;
using UnityEditor;
using System.Linq;

public class AnimationReverser
{
    [MenuItem("Assets/Reverse Animation Clip")]
    static void ReverseAnimationClip()
    {
        AnimationClip clip = Selection.activeObject as AnimationClip;
        if (clip == null)
        {
            Debug.LogError("Animation Clip을 선택해주세요.");
            return;
        }

        // 클립 복제
        string path = AssetDatabase.GetAssetPath(clip);
        string newPath = path.Replace(".anim", "_Reversed.anim");
        AssetDatabase.CopyAsset(path, newPath);
        AnimationClip newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(newPath);

        float clipLength = newClip.length;

        // 모든 커브 가져오기
        EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(newClip);

        foreach (var binding in bindings)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(newClip, binding);
            ObjectReferenceKeyframe[] reversed = new ObjectReferenceKeyframe[keyframes.Length];

            for (int i = 0; i < keyframes.Length; i++)
            {
                reversed[i] = new ObjectReferenceKeyframe
                {
                    time = clipLength - keyframes[keyframes.Length - 1 - i].time,
                    value = keyframes[keyframes.Length - 1 - i].value
                };
            }

            AnimationUtility.SetObjectReferenceCurve(newClip, binding, reversed);
        }

        EditorUtility.SetDirty(newClip);
        AssetDatabase.SaveAssets();
        Debug.Log($"반전된 클립 생성: {newPath}");
    }

    [MenuItem("Assets/Reverse Animation Clip", true)]
    static bool ValidateReverseAnimationClip()
    {
        return Selection.activeObject is AnimationClip;
    }
}
