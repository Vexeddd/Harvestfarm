using System;

public class ItemQueryRequestEvent
{
    public int item_id;

    public Action<int> callBack;
}
