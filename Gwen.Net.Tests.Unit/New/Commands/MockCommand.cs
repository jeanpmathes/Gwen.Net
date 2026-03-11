using Gwen.Net.New.Bindings;
using Gwen.Net.New.Commands;

namespace Gwen.Net.Tests.Unit.New.Commands;

public sealed class MockCommand : ICommand<Object>
{
    private readonly Slot<Boolean> canExecute = new(true);

    private Boolean willHoldExecution;
    private List<MockExecution> heldExecutions = [];

    public MockCommand()
    {
        CanExecute = Binding.To(canExecute).Parametrize<Object, Boolean>((_, value) => value);
    }

    /// <summary>
    /// The number of times <see cref="Execute"/> has been called.
    /// </summary>
    public Int32 ExecuteCount { get; private set; }

    /// <inheritdoc />
    public IValueSource<Object, Boolean> CanExecute { get; }

    public ICommandExecution Execute(Object argument)
    {
        ExecuteCount++;

        if (!willHoldExecution) return Command.Succeeded;

        MockExecution execution = new();
        heldExecutions.Add(execution);
        return execution;
    }

    public void SetCanExecute(Boolean value)
    {
        canExecute.SetValue(value);
    }

    public void HoldExecutionCompletion()
    {
        willHoldExecution = true;
    }

    public void CompleteExecution()
    {
        willHoldExecution = false;

        foreach (MockExecution execution in heldExecutions)
            execution.Complete(Status.Succeeded);

        heldExecutions = [];
    }
}

public sealed class MockExecution : ICommandExecution
{
    private readonly Slot<Status> status = new(Net.New.Commands.Status.Running);

    public IValueSource<Status> Status => status;

    public IValueSource<Single>? Progress => null;

    public void Dispose() {}

    public void Complete(Status completionStatus)
    {
        status.SetValue(completionStatus);
    }
}
