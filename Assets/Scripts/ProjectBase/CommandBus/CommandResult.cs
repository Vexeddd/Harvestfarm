
public struct CommandResult
{
    public bool Success;
    public string Message;

    public static CommandResult OK()
    {
        return new CommandResult { Success = true, Message = "³É¹¦" };
    }

    public static CommandResult Fail(string message)
    {
        return new CommandResult
        {
            Success = false,
            Message = message
        };
    }
}
