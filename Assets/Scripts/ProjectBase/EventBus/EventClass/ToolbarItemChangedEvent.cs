
public class ToolbarItemChangedEvent
{
    //public ItemData itemData
    //{
    //    get; private set;
    //}
    public InventoryItem inventoryItem { get; private set; }

    public ToolbarItemChangedEvent(InventoryItem item)
    {
        inventoryItem = item;
    }

    //public ToolbarItemChangedEvent(ItemData itemData)
    //{
    //    this.itemData = itemData;
    //}
}
