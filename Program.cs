using WorkflowEngine.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory data stores
var definitions = new Dictionary<string, WorkflowDefinition>();
var instances = new Dictionary<string, WorkflowInstance>();

// Create workflow definition
app.MapPost("/workflow", (WorkflowDefinition def) =>
{
    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Exactly one initial state is required.");

    if (definitions.ContainsKey(def.Id))
        return Results.BadRequest("Duplicate workflow ID.");

    definitions[def.Id] = def;
    return Results.Ok(def);
});

// Get workflow definition
app.MapGet("/workflow/{id}", (string id) =>
{
    return definitions.TryGetValue(id, out var def)
        ? Results.Ok(def)
        : Results.NotFound("Workflow not found.");
});

// Start a new workflow instance
app.MapPost("/instance/{definitionId}", (string definitionId) =>
{
    if (!definitions.TryGetValue(definitionId, out var def))
        return Results.NotFound("Workflow definition not found.");

    var initial = def.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
    if (initial == null)
        return Results.BadRequest("No enabled initial state found.");

    var instance = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        DefinitionId = def.Id,
        CurrentStateId = initial.Id
    };

    instances[instance.Id] = instance;
    return Results.Ok(instance);
});

// Execute an action
app.MapPost("/instance/{id}/action/{actionId}", (string id, string actionId) =>
{
    if (!instances.TryGetValue(id, out var instance))
        return Results.NotFound("Workflow instance not found.");

    var def = definitions[instance.DefinitionId];
    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);

    if (action == null || !action.Enabled)
        return Results.BadRequest("Invalid or disabled action.");

    if (!action.FromStates.Contains(instance.CurrentStateId))
        return Results.BadRequest("Action not allowed from current state.");

    var currentState = def.States.First(s => s.Id == instance.CurrentStateId);
    if (currentState.IsFinal)
        return Results.BadRequest("Cannot execute action from a final state.");

    var target = def.States.FirstOrDefault(s => s.Id == action.ToState);
    if (target == null || !target.Enabled)
        return Results.BadRequest("Invalid or disabled target state.");

    instance.CurrentStateId = target.Id;
    instance.History.Add(new WorkflowHistoryEntry
    {
        ActionId = action.Id,
        Timestamp = DateTime.UtcNow
    });
    return Results.Ok(instance);
});
//Infonetica Software Engineer Intern Take-Home Exercise by Mozeel Pradip Vanwani (23CS10047) IIT Kharagpur

// Get instance status
app.MapGet("/instance/{id}", (string id) =>
{
    return instances.TryGetValue(id, out var inst)
        ? Results.Ok(inst)
        : Results.NotFound("Workflow instance not found.");
});

// Delete a workflow definition
app.MapDelete("/workflow/{id}", (string id) =>
{
    if (!definitions.ContainsKey(id))
        return Results.NotFound("Workflow definition not found.");

    definitions.Remove(id);
    return Results.Ok($"Workflow definition '{id}' deleted.");
});

app.Run();
