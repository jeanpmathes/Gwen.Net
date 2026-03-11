using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Commands;

/// <summary>
///     Interface for commands. Commands are used to bind actions to controls.
/// </summary>
/// <typeparam name="TArgument">The type of argument passed to the command when executed.</typeparam>
public interface ICommand<in TArgument>
{
    /// <summary>
    ///     Whether the command can be executed.
    /// </summary>
    public IValueSource<TArgument, Boolean> CanExecute { get; }

    /// <summary>
    ///     Execute the command.
    /// </summary>
    public ICommandExecution Execute(TArgument argument);
}
