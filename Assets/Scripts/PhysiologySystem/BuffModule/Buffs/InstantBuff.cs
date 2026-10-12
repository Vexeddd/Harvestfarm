public class InstantBuff : BaseBuff
{
    public InstantBuff(BuffData data) : base(data) { }

    public override void Apply()
    {
        timeLeft = 0;

        //即时性的buff直接通过生理系统修改状态
        PhysiologySystem.Instance.vitalContainer.Get(buffData.vitalType).ModifyValue(buffData.value);
    }
}
