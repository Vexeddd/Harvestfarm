/// <summary>
/// ÃüÁî´¦ÀíÕß
/// </summary>
/// <typeparam name="TCommand"></typeparam>

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Handle(TCommand command);
}
