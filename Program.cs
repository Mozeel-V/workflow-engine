using WorkflowEngine.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// storing all workflow definitions in-memory
var definitions = new Dictionary<string, WorkflowDefinition>();

// storing all workflow instances in-memory
var instances = new Dictionary<string, WorkflowInstance>();

// creating the workflow definition
app.MapPost("/workflow", (WorkflowDefinition def) =>
{
    // validating: there must be exactly one initial state
    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Exactly one initial state is required.");

    // validating: prevent duplicate workflow definitions by ID
    if (definitions.ContainsKey(def.Id))
        return Results.BadRequest("Duplicate workflow ID.");

    // TODO: validate that all state IDs are unique
    // TODO: validate that all action IDs are unique
    // TODO: validate that all toStates and fromStates in actions exist in the state list

    definitions[def.Id] = def;
    return Results.Ok(def);
});

// retrieving an existing workflow definition by ID
app.MapGet("/workflow/{id}", (string id) =>
{
    return definitions.TryGetValue(id, out var def)
        ? Results.Ok(def)
        : Results.NotFound("Workflow not found.");
});

// starting a new workflow instance based on a given definition
app.MapPost("/instance/{definitionId}", (string definitionId) =>
{
    if (!definitions.TryGetValue(definitionId, out var def))
        return Results.NotFound("Workflow definition not found.");

    // finding the enabled initial state to start from
    var initial = def.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
    if (initial == null)
        return Results.BadRequest("No enabled initial state found.");

    // creating a new instance starting at the initial state
    var instance = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        DefinitionId = def.Id,
        CurrentStateId = initial.Id
    };

    instances[instance.Id] = instance;
    return Results.Ok(instance);
});

// executing an action on a given workflow instance
app.MapPost("/instance/{id}/action/{actionId}", (string id, string actionId) =>
{
    if (!instances.TryGetValue(id, out var instance))
        return Results.NotFound("Workflow instance not found.");

    var def = definitions[instance.DefinitionId];
    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);

    // validating: action must exist and be enabled
    if (action == null || !action.Enabled)
        return Results.BadRequest("Invalid or disabled action.");

    // validating: action must be allowed from the current state
    if (!action.FromStates.Contains(instance.CurrentStateId))
        return Results.BadRequest("Action not allowed from current state.");

    var currentState = def.States.First(s => s.Id == instance.CurrentStateId);
    // validating: cannot transition from a final state
    if (currentState.IsFinal)
        return Results.BadRequest("Cannot execute action from a final state.");

    // validating: target state must exist and be enabled
    var target = def.States.FirstOrDefault(s => s.Id == action.ToState);
    if (target == null || !target.Enabled)
        return Results.BadRequest("Invalid or disabled target state.");

    // performing the state transition and recording history
    instance.CurrentStateId = target.Id;
    instance.History.Add(new WorkflowHistoryEntry
    {
        ActionId = action.Id,
        Timestamp = DateTime.UtcNow
    });

    // TODO: optionally prevent loops or repeated states if needed
    // TODO: track user or source of action (for audit trail)

    return Results.Ok(instance);
});
//Infonetica Software Engineer Intern Take-Home Exercise by Mozeel Pradip Vanwani (23CS10047) IIT Kharagpur

// retrieving the current state and history of an instance
app.MapGet("/instance/{id}", (string id) =>
{
    return instances.TryGetValue(id, out var inst)
        ? Results.Ok(inst)
        : Results.NotFound("Workflow instance not found.");
});

// deleting a workflow definition by ID
app.MapDelete("/workflow/{id}", (string id) =>
{
    if (!definitions.ContainsKey(id))
        return Results.NotFound("Workflow definition not found.");

    definitions.Remove(id);
    return Results.Ok($"Workflow definition '{id}' deleted.");
});

// TODO: implement endpoint to list all workflow definitions
// TODO: implement endpoint to list all workflow instances
// TODO: allow updating or disabling existing actions or states
// TODO: persist data to local JSON/YAML file instead of memory

app.Run();
