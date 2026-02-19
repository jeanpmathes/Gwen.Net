using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Commands;

/// <summary>
/// Interface for commands. Commands are used to bind actions to controls.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Whether the command can be executed.
    /// </summary>
    public ReadOnlySlot<Boolean> CanExecute { get; }
    
    /// <summary>
    /// Execute the command.
    /// </summary>
    public ICommandExecution Execute();
}
