using System;
using System.Collections.Generic;


public static class CommandBus
{
    public static Dictionary<Type, object> _handler = new Dictionary<Type, object>();


    public static void Register<TCommand>(ICommandHandler<TCommand> commandHandler) where TCommand : ICommand
    {
        Type type = typeof(TCommand);
        if (_handler.TryGetValue(type, out var handle))
        {
            throw new Exception("命令处理器已经注册");
        }

        _handler.Add(type, commandHandler);
    }

    
    public static CommandResult Send<TCommand>(TCommand command) where TCommand : ICommand
    {
        Type type = typeof(TCommand);
        if (!_handler.TryGetValue(type, out var handle))
        {
            throw new Exception("没有找到处理者");
        }
        
        (_handler[type] as ICommandHandler<TCommand>).Handle(command);
        return CommandResult.OK();
    }
}
