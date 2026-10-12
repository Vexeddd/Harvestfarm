/// <summary>
/// 消耗血量命令
/// </summary>
public class CommandConsumeHP : ICommand
{
    public float amount;
}

/// <summary>
/// 消耗耐力命令
/// </summary>
public class CommandConsumeStamina : ICommand
{
    public float amount;
}

/// <summary>
/// 消耗饱食度命令
/// </summary>
public class CommandConsumeSatiely : ICommand
{
    public float amount;
}