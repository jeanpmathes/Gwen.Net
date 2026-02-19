using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Commands;

/// <summary>
/// Utility class for commands.
/// </summary>
public static class Command
{
    /// <summary>
    /// Get a successful command execution. This can be used for commands that execute synchronously and always succeed.
    /// </summary>
    public static ICommandExecution Succeeded { get; } = new SucceededExecution();
    
    /// <summary>
    /// Create a command from an action. The command will always be executable and will return a successful execution when executed.
    /// </summary>
    /// <param name="action">The action to execute when the command is executed.</param>
    /// <returns>A command that executes the given action.</returns>
    public static ICommand FromAction(Action action)
    {
        return new ActionCommand(action);
    }
    
    private class SucceededExecution : ICommandExecution
    {
        public ReadOnlySlot<Status> Status { get; } = new(Commands.Status.Succeeded);
        
        public ReadOnlySlot<Single>? Progress => null;

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
    
    private class ActionCommand(Action action) : ICommand
    {
        public ReadOnlySlot<Boolean> CanExecute { get; } = new(true);

        public ICommandExecution Execute()
        {
            action();
            
            return Succeeded;
        }
    }
}
