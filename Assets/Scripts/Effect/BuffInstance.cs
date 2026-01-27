using UnityEngine;

/// <summary>
/// 유닛에 적용된 버프의 런타임 인스턴스
/// 지속 시간과 스택 수를 추적
/// </summary>
[System.Serializable]
public class BuffInstance
{
    public BuffData buffData;
    public int remainingDuration;
    public int stacks;

    public BuffInstance(BuffData data)
    {
        buffData = data;
        remainingDuration = data.duration;
        stacks = 1;
    }

    /// <summary>
    /// 턴 경과 처리
    /// </summary>
    /// <returns>버프가 만료되면 true</returns>
    public bool TickDuration()
    {
        if (buffData.duration <= 0) return false; // 영구 버프

        remainingDuration--;
        return remainingDuration <= 0;
    }

    /// <summary>
    /// 스택 추가
    /// </summary>
    /// <returns>스택 추가 성공 여부</returns>
    public bool AddStack()
    {
        if (stacks >= buffData.maxStacks) return false;

        stacks++;
        return true;
    }

    /// <summary>
    /// 지속 시간 갱신
    /// </summary>
    public void RefreshDuration()
    {
        remainingDuration = buffData.duration;
    }
}
