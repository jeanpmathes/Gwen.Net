namespace Gwen.Net.New.Commands;

/// <summary>
/// The status of a command execution.
/// </summary>
public enum Status
{
    /// <summary>
    /// The command is currently running.
    /// </summary>
    Running,
    
    /// <summary>
    /// The command has completed successfully.
    /// </summary>
    Succeeded,
    
    /// <summary>
    /// The command has failed.
    /// </summary>
    Failed,
    
    /// <summary>
    /// The command has been canceled.
    /// </summary>
    Canceled
}
