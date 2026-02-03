using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class AfterImage_Effect_Manager : MonoBehaviour
{
    [Header("풀링 설정")]
    [SerializeField] private List<GameObject> afterImageObjects;
    [SerializeField] private float afterimageAlpha = 0.8f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Color tintColor = Color.white;

    private Dictionary<GameObject, SpriteRenderer> afterImageDict;
    private Queue<GameObject> availablePool;

    private void Awake()
    {
        afterImageDict = new Dictionary<GameObject, SpriteRenderer>();
        availablePool = new Queue<GameObject>();

        foreach (var obj in afterImageObjects)
        {
            afterImageDict.Add(obj, obj.GetComponent<SpriteRenderer>());
            availablePool.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    public void CreateAfterimage(Sprite sprite)
    {
        if (sprite == null || availablePool.Count == 0) return;

        var obj = availablePool.Dequeue();
        var sr = afterImageDict[obj];

        obj.SetActive(true);

        sr.sprite = sprite;
        sr.color = new Color(tintColor.r, tintColor.g, tintColor.b, afterimageAlpha);

        sr.DOFade(0f, fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                obj.SetActive(false);
                availablePool.Enqueue(obj);
            });
    }
}
