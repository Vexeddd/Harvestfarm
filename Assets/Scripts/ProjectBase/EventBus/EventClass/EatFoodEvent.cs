public class EatFoodEvent
{
    public ItemData itemData { get; private set; }

    public EatFoodEvent(ItemData item)
    {
        itemData = item;
    }
}