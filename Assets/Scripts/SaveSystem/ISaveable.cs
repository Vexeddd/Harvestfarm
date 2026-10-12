/// <summary>
/// Implemented by each system that owns runtime state to persist.
/// SaveManager collects all saveables and calls OnSave/OnLoad in order.
/// </summary>
public interface ISaveable
{
    void OnSave(SaveData data);
    void OnLoad(SaveData data);
}
