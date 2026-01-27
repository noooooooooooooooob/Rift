using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// 버튼 호버 시 텍스트 색상 변경 + 포인터 애니메이션 효과
/// </summary>
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Reference")]
    public TMP_Text targetText;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("Pointer")]
    public RectTransform pointer;           // 왼쪽 포인터 이미지
    public float pointerMoveDistance = 20f; // 왔다갔다 거리
    public float pointerMoveSpeed = 0.7f;   // 왔다갔다 속도
    public float pointerMinAlpha = 0.1f;    // 최소 불투명도
    public float pointerMaxAlpha = 0.8f;      // 최대 불투명도

    private Vector3 pointerOriginalPos;
    private Image pointerImage;
    private Tween pointerMoveTween;
    private Tween pointerFadeTween;

    void Start()
    {
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TMP_Text>();
        }

        if (targetText != null)
        {
            targetText.color = normalColor;
        }

        if (pointer != null)
        {
            pointerOriginalPos = pointer.localPosition;
            pointerImage = pointer.GetComponent<Image>();
            if (pointerImage != null)
            {
                pointerImage.enabled = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText != null)
        {
            targetText.color = hoverColor;
        }

        if (pointer != null)
        {
            pointer.localPosition = pointerOriginalPos;
            if (pointerImage != null)
            {
                pointerImage.enabled = true;
            }

            // 양옆으로 왔다갔다 애니메이션
            pointerMoveTween?.Kill();
            pointerMoveTween = pointer.DOLocalMoveX(pointerOriginalPos.x + pointerMoveDistance, pointerMoveSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            // 불투명도 깜빡임 애니메이션
            if (pointerImage != null)
            {
                pointerFadeTween?.Kill();
                pointerImage.color = new Color(pointerImage.color.r, pointerImage.color.g, pointerImage.color.b, pointerMaxAlpha);
                pointerFadeTween = pointerImage.DOFade(pointerMinAlpha, pointerMoveSpeed)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetText != null)
        {
            targetText.color = normalColor;
        }

        if (pointer != null)
        {
            pointerMoveTween?.Kill();
            pointerFadeTween?.Kill();
            if (pointerImage != null)
            {
                pointerImage.enabled = false;
            }
        }
    }

    void OnDisable()
    {
        pointerMoveTween?.Kill();
        pointerFadeTween?.Kill();
    }
}
