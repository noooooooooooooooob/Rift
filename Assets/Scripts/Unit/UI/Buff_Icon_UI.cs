using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Buff_Icon_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text stackText;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private BuffInstance buffInstance;

    public void Setup(BuffInstance instance)
    {
        buffInstance = instance;

        if (instance.buffData.icon != null)
            iconImage.sprite = instance.buffData.icon;

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (buffInstance == null) return;

        // Count (남은 턴)
        if (countText != null)
        {
            if (buffInstance.buffData.duration > 0)
                countText.text = buffInstance.remainingDuration.ToString();
            else
                countText.text = "";
        }

        // Stack
        if (stackText != null)
        {
            if (buffInstance.buffData.maxStacks > 1)
                stackText.text = buffInstance.stacks.ToString();
            else
                stackText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel == null || buffInstance == null) return;

        tooltipPanel.SetActive(true);

        if (nameText != null)
            nameText.text = buffInstance.buffData.buffName;

        if (descriptionText != null)
            descriptionText.text = buffInstance.buffData.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    public BuffData GetBuffData() => buffInstance?.buffData;
}
