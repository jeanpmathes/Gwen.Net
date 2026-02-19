using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Commands;

/// <summary>
/// Represents the execution of a command.
/// </summary>
public interface ICommandExecution : IDisposable
{
    /// <summary>
    /// The status of the command execution.
    /// </summary>
    public ReadOnlySlot<Status> Status { get; }
    
    /// <summary>
    /// The progress of the command execution, as a value between 0 and 1. Null if the command does not report progress.
    /// </summary>
    public ReadOnlySlot<Single>? Progress { get; }
}
