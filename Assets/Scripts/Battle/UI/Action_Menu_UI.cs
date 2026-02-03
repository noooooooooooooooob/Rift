using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class Action_Menu_UI : MonoBehaviour
{
    public Button ultimateButton;
    [Header("Panels")]
    public GameObject ActionMenuPanel;
    public GameObject ReturnMenuPanel;

    [Header("Action Display - Icons")]
    public Image prevActionIcon;      // 위쪽 (이전 액션, 투명)
    public Image currentActionIcon;   // 중앙 (현재 선택)
    public Image nextActionIcon;      // 아래쪽 (다음 액션, 투명)

    [Header("Action Display - Text")]
    public TMP_Text currentActionName; // 중앙만 텍스트 표시
    public TMP_Text actionCostText;

    [Header("Navigation Buttons")]
    public Button upButton;
    public Button downButton;
    public Button confirmButton;

    [Header("Return Panel")]
    public Button returnButton;

    [Header("Animation Settings")]
    public float transitionDuration = 0.2f;
    public float previewAlpha = 0.4f;
    public float spacing = 100f; // 슬롯 간 간격

    [Header("Data")]
    public List<ActionData> actionDataList = new List<ActionData>();

    private int currentIndex = 0;
    private bool isAnimating = false;
    private Vector3 prevOriginalPos;
    private Vector3 currentOriginalPos;
    private Vector3 nextOriginalPos;

    void Awake()
    {
        ultimateButton.interactable = false;
        // 원래 위치 저장
        if (prevActionIcon != null)
            prevOriginalPos = prevActionIcon.transform.localPosition;
        if (currentActionIcon != null)
            currentOriginalPos = currentActionIcon.transform.localPosition;
        if (nextActionIcon != null)
            nextOriginalPos = nextActionIcon.transform.localPosition;
    }

    public void ShowActionMenu()
    {
        ActionMenuPanel.SetActive(true);
        currentIndex = 0;
        UpdateDisplay();
    }

    public void HideActionMenu()
    {
        ActionMenuPanel.SetActive(false);
    }

    public void ShowReturnMenu()
    {
        ActionMenuPanel.SetActive(false);
        ReturnMenuPanel.SetActive(true);
    }

    public void HideReturnMenu()
    {
        ReturnMenuPanel.SetActive(false);
    }

    public void HideAllMenus()
    {
        ActionMenuPanel.SetActive(false);
        ReturnMenuPanel.SetActive(false);
    }

    public bool IsMenuActive()
    {
        return ActionMenuPanel.activeSelf;
    }

    public ActionType GetCurrentActionType()
    {
        if (actionDataList.Count == 0)
            return ActionType.Move;

        return actionDataList[currentIndex].actionType;
    }

    /// <summary>
    /// 다음 액션으로 이동 (아래 방향)
    /// </summary>
    public void SelectNext()
    {
        if (isAnimating || actionDataList.Count <= 1)
            return;

        isAnimating = true;
        PlaySlideAnimation(1);
    }

    /// <summary>
    /// 이전 액션으로 이동 (위 방향)
    /// </summary>
    public void SelectPrevious()
    {
        if (isAnimating || actionDataList.Count <= 1)
            return;

        isAnimating = true;
        PlaySlideAnimation(-1);
    }

    /// <summary>
    /// 슬라이드 애니메이션 재생
    /// </summary>
    /// <param name="direction">1: 아래로 이동 (다음 선택), -1: 위로 이동 (이전 선택)</param>
    private void PlaySlideAnimation(int direction)
    {
        float moveDistance = spacing * direction;

        // 모든 아이콘 이동 애니메이션
        Sequence sequence = DOTween.Sequence();

        // 위 슬롯: 이동 + 페이드
        if (prevActionIcon != null)
        {
            sequence.Join(prevActionIcon.transform.DOLocalMoveY(
                prevOriginalPos.y - moveDistance, transitionDuration));

            if (direction > 0) // 아래로 이동 시, 위 슬롯은 사라짐
            {
                sequence.Join(prevActionIcon.DOFade(0f, transitionDuration));
            }
            else // 위로 이동 시, 위 슬롯은 현재 슬롯이 됨
            {
                sequence.Join(prevActionIcon.DOFade(1f, transitionDuration));
            }
        }

        // 중앙 슬롯: 이동 + 페이드
        if (currentActionIcon != null)
        {
            sequence.Join(currentActionIcon.transform.DOLocalMoveY(
                currentOriginalPos.y - moveDistance, transitionDuration));

            // 중앙은 미리보기 투명도로 변함
            sequence.Join(currentActionIcon.DOFade(previewAlpha, transitionDuration));
        }

        // 아래 슬롯: 이동 + 페이드
        if (nextActionIcon != null)
        {
            sequence.Join(nextActionIcon.transform.DOLocalMoveY(
                nextOriginalPos.y - moveDistance, transitionDuration));

            if (direction > 0) // 아래로 이동 시, 아래 슬롯은 현재 슬롯이 됨
            {
                sequence.Join(nextActionIcon.DOFade(1f, transitionDuration));
            }
            else // 위로 이동 시, 아래 슬롯은 사라짐
            {
                sequence.Join(nextActionIcon.DOFade(0f, transitionDuration));
            }
        }

        // 애니메이션 완료 후 인덱스 변경 및 디스플레이 리셋
        sequence.OnComplete(() =>
        {
            // 인덱스 업데이트 (순환)
            currentIndex = (currentIndex + direction + actionDataList.Count) % actionDataList.Count;

            // 위치 리셋 및 디스플레이 업데이트
            ResetPositions();
            UpdateDisplay();
            isAnimating = false;
        });
    }

    /// <summary>
    /// 모든 슬롯 위치를 원래대로 리셋
    /// </summary>
    private void ResetPositions()
    {
        if (prevActionIcon != null)
            prevActionIcon.transform.localPosition = prevOriginalPos;
        if (currentActionIcon != null)
            currentActionIcon.transform.localPosition = currentOriginalPos;
        if (nextActionIcon != null)
            nextActionIcon.transform.localPosition = nextOriginalPos;
    }

    /// <summary>
    /// 현재 인덱스를 기준으로 3개 슬롯 업데이트
    /// </summary>
    public void UpdateDisplay()
    {
        if (actionDataList.Count == 0)
            return;

        int prevIndex = (currentIndex - 1 + actionDataList.Count) % actionDataList.Count;
        int nextIndex = (currentIndex + 1) % actionDataList.Count;

        // 이전 액션 (위쪽, 투명)
        if (prevActionIcon != null && actionDataList.Count > 1)
        {
            prevActionIcon.sprite = actionDataList[prevIndex].actionIcon;
            SetImageAlpha(prevActionIcon, previewAlpha);
            prevActionIcon.gameObject.SetActive(true);
        }
        else if (prevActionIcon != null)
        {
            prevActionIcon.gameObject.SetActive(false);
        }

        // 현재 액션 (중앙, 불투명)
        if (currentActionIcon != null)
        {
            currentActionIcon.sprite = actionDataList[currentIndex].actionIcon;
            SetImageAlpha(currentActionIcon, 1f);
        }

        if (currentActionName != null)
        {
            currentActionName.text = actionDataList[currentIndex].actionName;
        }

        if (actionCostText != null)
        {
            actionCostText.text = actionDataList[currentIndex].actionCost.ToString();
        }

        // 다음 액션 (아래쪽, 투명)
        if (nextActionIcon != null && actionDataList.Count > 1)
        {
            nextActionIcon.sprite = actionDataList[nextIndex].actionIcon;
            SetImageAlpha(nextActionIcon, previewAlpha);
            nextActionIcon.gameObject.SetActive(true);
        }
        else if (nextActionIcon != null)
        {
            nextActionIcon.gameObject.SetActive(false);
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
