public class ForeverBuff : BaseBuff
{
    public ForeverBuff(BuffData data) : base(data)
    {
    }

    public override bool OnUpdate(float deltaTime)
    {
        //饥饿感, 100  -1   -1 ..... 0 -hp  ...dead
        //饥渴感, 100  -1   -1  ..... 0 ........
        //耐力, 缓慢增加耐力......

        PhysiologySystem.Instance.vitalContainer.Get(buffData.vitalType).ModifyValue(buffData.value);

        return false; // 一直不结束Update，以实现永久
    }
}
