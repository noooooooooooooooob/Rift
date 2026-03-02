using UnityEngine;

public class Afterimage_Effect : MonoBehaviour
{
    [Header("잔상 설정")]
    [SerializeField] private Sprite afterimageSprite;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private bool trackSpriteChanges = true;
    public AfterImage_Effect_Manager afterimageManager;

    private Sprite previousSprite;

    private void Start()
    {
        if (targetSpriteRenderer != null)
        {
            previousSprite = targetSpriteRenderer.sprite;
        }
    }

    private void LateUpdate()
    {
        if (!trackSpriteChanges || targetSpriteRenderer == null) return;

        if (targetSpriteRenderer.sprite != previousSprite)
        {
            // 스프라이트가 변경되면 이전 스프라이트로 잔상 생성
            if (previousSprite != null)
            {
                afterimageManager.CreateAfterimage(previousSprite);
            }
            previousSprite = targetSpriteRenderer.sprite;
        }
    }

    private void OnDisable()
    {
        CreateAfterimage();
    }

    public void CreateAfterimage()
    {
        if (trackSpriteChanges && targetSpriteRenderer != null)
        {
            afterimageManager.CreateAfterimage(targetSpriteRenderer.sprite);
        }
        else if (afterimageSprite != null)
        {
            afterimageManager.CreateAfterimage(afterimageSprite);
        }
    }
}
