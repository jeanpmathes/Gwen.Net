using Gwen.Net.New.Commands;

namespace Gwen.Net.Tests.Unit.New.Commands;

public class CommandTests
{
    private static readonly Object placeholder = new();

    [Fact]
    public void Succeeded_HasSucceededStatus()
    {
        ICommandExecution execution = Command.Succeeded;

        Assert.Equal(Status.Succeeded, execution.Status.GetValue());
    }

    [Fact]
    public void FromAction_CanExecute_IsTrue()
    {
        ICommand<Object> command = Command.FromAction(() => {});

        Assert.True(command.CanExecute.GetValue(placeholder));
    }

    [Fact]
    public void FromAction_Execute_InvokesAction()
    {
        Boolean invoked = false;
        ICommand<Object> command = Command.FromAction(() => invoked = true);

        command.Execute(placeholder);

        Assert.True(invoked);
    }

    [Fact]
    public void FromAction_Execute_CanBeCalledMultipleTimes()
    {
        Int32 count = 0;
        ICommand<Object> command = Command.FromAction(() => count++);

        command.Execute(placeholder);
        command.Execute(placeholder);
        command.Execute(placeholder);

        Assert.Equal(expected: 3, count);
    }

    [Fact]
    public void FromActionWithArgument_Execute_PassesArgumentToAction()
    {
        Object? receivedArgument = null;
        ICommand<String> command = Command.FromAction<String>(arg => receivedArgument = arg);

        const String argument = "Test Argument";
        command.Execute(argument);

        Assert.Equal(argument, receivedArgument);
    }
}
