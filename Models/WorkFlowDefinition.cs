namespace WorkflowEngine.Models;

public class WorkflowDefinition
{
    public string Id { get; set; } = string.Empty;
    public List<State> States { get; set; } = new();
    public List<ActionTransition> Actions { get; set; } = new();
}
