using UnityEngine;
using System.Collections.Generic;

public class Buff_Container_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EffectHandler effectHandler;
    [SerializeField] private List<Buff_Icon_UI> buffIcons = new List<Buff_Icon_UI>();

    private void Start()
    {
        if (effectHandler == null)
            effectHandler = GetComponentInParent<Unit>()?.GetComponent<EffectHandler>();

        // 시작 시 모두 비활성화
        HideAll();
    }

    public void RefreshBuffs()
    {
        if (effectHandler == null) return;

        var activeBuffs = effectHandler.GetActiveBuffs();

        for (int i = 0; i < buffIcons.Count; i++)
        {
            if (i < activeBuffs.Count)
            {
                buffIcons[i].Setup(activeBuffs[i]);
                buffIcons[i].gameObject.SetActive(true);
            }
            else
            {
                buffIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideAll()
    {
        foreach (var icon in buffIcons)
        {
            if (icon != null)
                icon.gameObject.SetActive(false);
        }
    }

    public void ClearAll()
    {
        HideAll();
    }
}
