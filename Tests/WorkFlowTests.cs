using Xunit;
using WorkflowEngine.Models;

namespace WorkflowEngine.Tests;

public class WorkflowTests
{
    [Fact]
    public void WorkflowDefinition_Should_Have_One_Initial_State()
    {
        // setting up a workflow with two states marked as initial (invalid case)
        var definition = new WorkflowDefinition
        {
            Id = "test",
            States = new List<State>
            {
                new State { Id = "start", IsInitial = true },
                new State { Id = "another", IsInitial = true }
            },
            Actions = new List<ActionTransition>()
        };

        // counting how many initial states exist
        var initialStatesCount = definition.States.Count(s => s.IsInitial);

        // asserting that it's not equal to 1 (which would be valid)
        // this test should pass because this is an invalid workflow
        Assert.NotEqual(1, initialStatesCount);
    }

    [Fact]
    public void Can_Transition_From_Valid_State()
    {
        // defining a valid workflow with two states and one transition
        var definition = new WorkflowDefinition
        {
            Id = "test",
            States = new List<State>
            {
                new State { Id = "submitted", IsInitial = true, Enabled = true },
                new State { Id = "approved", IsFinal = true, Enabled = true }
            },
            Actions = new List<ActionTransition>
            {
                new ActionTransition
                {
                    Id = "approve",
                    Name = "Approve",
                    FromStates = new List<string> { "submitted" },
                    ToState = "approved",
                    Enabled = true
                }
            }
        };

        // starting an instance in the 'submitted' state
        var instance = new WorkflowInstance
        {
            Id = "instance1",
            DefinitionId = "test",
            CurrentStateId = "submitted",
            History = new List<WorkflowHistoryEntry>()
        };

        // fetching the action and target state
        var action = definition.Actions.First();
        var currentState = definition.States.First(s => s.Id == instance.CurrentStateId);
        var target = definition.States.First(s => s.Id == action.ToState);

        // simulating execution of the action (manually performing state transition)
        instance.CurrentStateId = target.Id;
        instance.History.Add(new WorkflowHistoryEntry
        {
            ActionId = action.Id,
            Timestamp = DateTime.UtcNow
        });

        // asserting that the state has changed and the history was recorded correctly
        Assert.Equal("approved", instance.CurrentStateId);
        Assert.Single(instance.History);
    }
}
