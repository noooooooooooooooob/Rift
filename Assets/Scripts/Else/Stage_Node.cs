using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum Stage_Type
{
    BATTLE,
    EVENT,
    SHOP,
    REST,
    BOSS,
    START
}
[System.Serializable]
public class NodeConnection
{
    public Stage_Node targetNode;
    public LineRenderer connectionLine; // 시각적 연결선
}

public class Stage_Node : MonoBehaviour, IPointerDownHandler
{
    [Header("Node Info")]
    public Stage_Type nodeType;
    public Vector2 gridPosition; // 맵상 논리적 위치 (x=레이어, y=같은레이어내순서)

    [Header("State")]
    public bool isUnlocked = false;
    public bool isCompleted = false;
    public bool isCurrentNode = false;

    [Header("Connections")]
    public List<NodeConnection> nextNodes = new List<NodeConnection>();

    [Header("Visuals")]
    public SpriteRenderer nodeSprite;
    void Awake()
    {
        nodeSprite = GetComponent<SpriteRenderer>();
    }
    public void SetVisual(Stage_Type type)
    {
        nodeType = type;

        // 노드 아이콘 설정
        switch (nodeType)
        {
            case Stage_Type.BATTLE: nodeSprite.sprite = Stage_Manager.instance.battleIcon; break;
            case Stage_Type.SHOP: nodeSprite.sprite = Stage_Manager.instance.shopIcon; break;
            case Stage_Type.EVENT: nodeSprite.sprite = Stage_Manager.instance.eventIcon; break;
            case Stage_Type.BOSS: nodeSprite.sprite = Stage_Manager.instance.bossIcon; break;
            case Stage_Type.START: nodeSprite.sprite = Stage_Manager.instance.startIcon; break;
        }

        // 상태별 색상
        Color targetColor;
        if (isCurrentNode)
            targetColor = Stage_Manager.instance.currentColor;
        else if (isCompleted)
            targetColor = Stage_Manager.instance.completedColor;
        else if (isUnlocked)
            targetColor = Stage_Manager.instance.unlockedColor;
        else
            targetColor = Stage_Manager.instance.lockedColor;

        // 투명도 설정
        targetColor.a = isUnlocked ? 1.0f : 0.5f;
        nodeSprite.color = targetColor;
    }
    public void UpdateVisual()
    {
        // 상태별 색상
        Color targetColor;
        if (isCurrentNode)
            targetColor = Stage_Manager.instance.currentColor;
        else if (isCompleted)
            targetColor = Stage_Manager.instance.completedColor;
        else if (isUnlocked)
            targetColor = Stage_Manager.instance.unlockedColor;
        else
            targetColor = Stage_Manager.instance.lockedColor;

        // 투명도 설정
        targetColor.a = isUnlocked ? 1.0f : 0.5f;
        nodeSprite.color = targetColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            Stage_Manager.instance.SelectNode(this);
        }
    }
}
