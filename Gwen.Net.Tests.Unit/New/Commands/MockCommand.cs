using Gwen.Net.New.Bindings;
using Gwen.Net.New.Commands;

namespace Gwen.Net.Tests.Unit.New.Commands;

/// <summary>
/// A test double for <see cref="ICommand"/> that records how many times it has been executed.
/// </summary>
public sealed class MockCommand : ICommand
{
    /// <summary>
    /// The number of times <see cref="Execute"/> has been called.
    /// </summary>
    public Int32 ExecuteCount { get; private set; }

    /// <inheritdoc />
    public ReadOnlySlot<Boolean> CanExecute { get; } = new(true);

    /// <inheritdoc />
    public ICommandExecution Execute()
    {
        ExecuteCount++;

        return Command.Succeeded;
    }
}
