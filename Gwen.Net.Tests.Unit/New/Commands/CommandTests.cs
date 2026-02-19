using Gwen.Net.New.Commands;

namespace Gwen.Net.Tests.Unit.New.Commands;

public class CommandTests
{
    [Fact]
    public void Succeeded_HasSucceededStatus()
    {
        ICommandExecution execution = Command.Succeeded;

        Assert.Equal(Status.Succeeded, execution.Status.GetValue());
    }

    [Fact]
    public void FromAction_CanExecute_IsTrue()
    {
        ICommand command = Command.FromAction(() => { });

        Assert.True(command.CanExecute.GetValue());
    }

    [Fact]
    public void FromAction_Execute_InvokesAction()
    {
        var invoked = false;
        ICommand command = Command.FromAction(() => invoked = true);

        command.Execute();

        Assert.True(invoked);
    }

    [Fact]
    public void FromAction_Execute_CanBeCalledMultipleTimes()
    {
        var count = 0;
        ICommand command = Command.FromAction(() => count++);

        command.Execute();
        command.Execute();
        command.Execute();

        Assert.Equal(expected: 3, count);
    }
}
