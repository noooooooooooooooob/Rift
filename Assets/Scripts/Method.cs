using UnityEngine;

public static class Method
{
    // private const float DEF_K = 100f; // 방어력 감소 상수

    // // ========== 1. HIT(회피) 판정 ==========
    // public static bool IsHit(Unit attacker, Unit defender)
    // {
    //     // 현재는 명중 100%, 회피만 적용
    //     float evasion = defender.stats.EV; // 0~1

    //     // Random.value < EV 이면 회피
    //     if (Random.value < evasion)
    //         return false;

    //     return true;
    // }

    // // ========== 2. 유효 방어력 계산(관통 적용) ==========
    // public static float GetEffectiveDefense(Unit attacker, Unit defender)
    // {
    //     float def = defender.stats.DEF;
    //     float ap  = attacker.stats.AP;

    //     // 최소 0
    //     return Mathf.Max(0f, def - ap);
    // }

    // // ========== 3. 방어력 기반 피해 감소율 ==========
    // public static float GetDefenseReduction(float effectiveDef)
    // {
    //     // DEF / (DEF + K)
    //     return effectiveDef / (effectiveDef + DEF_K);
    // }

    // // ========== 4. DR(Damage Reduction) 결합 ==========
    // public static float GetTotalReduction(Unit attacker, Unit defender)
    // {
    //     float effectiveDef = GetEffectiveDefense(attacker, defender);
    //     float defReduction = GetDefenseReduction(effectiveDef); // 0~1
        
    //     float dr = defender.stats.DR; // 0~0.99

    //     // (1 - DEF) * (1 - DR) 조합
    //     float total = 1f - (1f - defReduction) * (1f - dr);

    //     return Mathf.Clamp(total, 0f, 0.99f);
    // }

    // // ========== 5. 최종 데미지 계산 ==========
    // public static int CalculateDamage(Unit attacker, Unit defender, float multiplier = 1f, float flatBonus = 0f)
    // {
    //     float atk = attacker.stats.ATK;

    //     // 기본 데미지
    //     float rawDamage = atk * multiplier + flatBonus;

    //     // 총 피해 감소율
    //     float reduction = GetTotalReduction(attacker, defender);

    //     float final = rawDamage * (1f - reduction);

    //     // 최소 1
    //     return Mathf.Max(1, Mathf.RoundToInt(final));
    // }

    // // ========== 6. 생명력 흡수 ==========
    // public static int CalculateLifeSteal(Unit attacker, int finalDamage)
    // {
    //     float ls = attacker.stats.LS;  // 0~1
    //     if (ls <= 0f) return 0;

    //     return Mathf.RoundToInt(finalDamage * ls);
    // }

    // // ========== 7. 반격 판정 ==========
    // public static bool TryCounter(Unit defender)
    // {
    //     float ca = defender.stats.CA;  // 0~1
    //     return Random.value < ca;
    // }

    // ========== 8. 레이어 재귀적 설정 ==========
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
