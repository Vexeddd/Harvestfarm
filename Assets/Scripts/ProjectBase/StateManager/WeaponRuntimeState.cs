public class WeaponRuntimeState : ItemRuntimeState
{
    public float current_duability;  //ÎäÆ÷ÄÍ¾Ã¶È
    public int current_ammo;   //ÎäÆ÷µ¯Ò©

    public WeaponRuntimeState(string uuid)
    {
        this.uuid = uuid;
    }

}