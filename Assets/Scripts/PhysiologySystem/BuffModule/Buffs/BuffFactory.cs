using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffFactory
{
    public static BaseBuff CreatBaseBuff(BuffData buffData)
    {
        switch (buffData.buffType)
        {
            case BuffType.Instant:
                return new InstantBuff(buffData);
            case BuffType.OverTime:
                return new OvertimeBuff(buffData);
            case BuffType.Forever:
                return new ForeverBuff(buffData);
            default:
                return new BaseBuff(buffData);
        }
    }
}
