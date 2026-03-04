using Gwen.Net.New.Bindings;
using Gwen.Net.New.Commands;

namespace Gwen.Net.Tests.Unit.New.Commands;

/// <summary>
/// A test double for <see cref="ICommand{TArgument}"/> that records how many times it has been executed.
/// </summary>
public sealed class MockCommand : ICommand<Object>
{
    /// <summary>
    /// The number of times <see cref="Execute"/> has been called.
    /// </summary>
    public Int32 ExecuteCount { get; private set; }

    /// <inheritdoc />
    public IValueSource<Object, Boolean> CanExecute { get; } = Binding.Constant<Object, Boolean>(true);

    public ICommandExecution Execute(Object argument)
    {
        ExecuteCount++;

        return Command.Succeeded;
    }
}
