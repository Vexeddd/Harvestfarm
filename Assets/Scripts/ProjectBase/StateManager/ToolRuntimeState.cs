

public class ToolRuntimeState : ItemRuntimeState
{
    public int current_duration;
    public int current_water;

    public ToolRuntimeState(string uuid) 
    {
        this.uuid = uuid;
    }
}
