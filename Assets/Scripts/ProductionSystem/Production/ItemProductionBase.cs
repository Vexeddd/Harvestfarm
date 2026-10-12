
public class ItemProductionBase
{
    public int item_id; //物品id
    public int count; //数量 

    public bool IsEmpty => item_id == 0 || count == 0;
}
