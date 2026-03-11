using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Commands;

/// <summary>
///     Utility class for commands.
/// </summary>
public static class Command
{
    /// <summary>
    ///     Get a successful command execution. This can be used for commands that execute synchronously and always succeed.
    /// </summary>
    public static ICommandExecution Succeeded { get; } = new SucceededExecution();

    /// <summary>
    ///     Create a command from an action. The command will always be executable and will return a successful execution when
    ///     executed.
    /// </summary>
    /// <param name="action">The action to execute when the command is executed.</param>
    /// <returns>A command that executes the given action.</returns>
    public static ICommand<Object> FromAction(Action action)
    {
        return new ActionCommand(action);
    }

    /// <summary>
    ///     Create a command from an action with an argument. The command will always be executable and will return a
    ///     successful execution when executed.
    /// </summary>
    /// <param name="action">
    ///     The action to execute when the command is executed. The argument passed to the command will be
    ///     passed to the action.
    /// </param>
    /// <typeparam name="TArgument">The type of the argument passed to the command and the action.</typeparam>
    /// <returns>A command that executes the given action with an argument.</returns>
    public static ICommand<TArgument> FromAction<TArgument>(Action<TArgument> action)
    {
        return new ActionCommand<TArgument>(action);
    }

    private class SucceededExecution : ICommandExecution
    {
        public IValueSource<Status> Status { get; } = new Slot<Status>(Commands.Status.Succeeded);

        public IValueSource<Single>? Progress => null;

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }

    private class ActionCommand(Action action) : ICommand<Object>
    {
        public IValueSource<Object, Boolean> CanExecute { get; } = Binding.Constant<Object, Boolean>(true);

        public ICommandExecution Execute(Object argument)
        {
            action();

            return Succeeded;
        }
    }

    private class ActionCommand<TArgument>(Action<TArgument> action) : ICommand<TArgument>
    {
        public IValueSource<TArgument, Boolean> CanExecute { get; } = Binding.Constant<TArgument, Boolean>(true);

        public ICommandExecution Execute(TArgument argument)
        {
            action(argument);

            return Succeeded;
        }
    }
}
