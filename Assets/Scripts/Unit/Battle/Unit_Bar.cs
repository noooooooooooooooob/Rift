using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유닛의 HP 바와 UP 바를 컨트롤하는 UI 컴포넌트
/// Unit에서 HP/UP 변경 시 호출
/// </summary>
public class Unit_Bar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider bar;
    public TextMeshProUGUI text;
    public void UpdateBar(float currentValue, float maxValue)
    {
        if (bar != null && maxValue > 0)
        {
            bar.value = currentValue / maxValue;
            if (text != null)
            {
                text.text = $"{currentValue} / {maxValue}";
            }
        }
    }
}
