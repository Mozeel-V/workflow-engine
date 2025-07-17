# workflow-engine

[![C#](https://img.shields.io/badge/language-CSharp-blue.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/framework-.NET_9.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/hosted_on-GitHub-black?logo=github)](https://github.com/Mozeel-V/workflow-engine)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg)](LICENSE)

This project implements a minimal configurable workflow engine backend using ASP.NET Core (.NET 9) and C#. The system allows you to define workflows with states and actions, start instances from those definitions, execute transitions, and track workflow history.

---

## 🚀 Features

- Define workflow definitions with states and transitions (actions)
- Start workflow instances from definitions
- Execute transitions to move between states
- Validate transitions based on rules
- Track action history per instance
- In-memory data storage (no database required)
- Minimal API design using .NET 9

---

## 🛠️ Getting Started
### ✅ Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- Postman or curl for testing
- Git

### ▶️ Running the Application

```bash
git clone https://github.com/Mozeel-V/workflow-engine.git
cd workflow-engine
dotnet run
```

Server will start at `http://localhost:5044` or a nearby port.

---

## 📬 API Endpoints

| Method | Endpoint                                | Description                       |
|--------|------------------------------------------|-----------------------------------|
| POST   | `/workflow`                             | Create a new workflow definition  |
| GET    | `/workflow/{id}`                        | Retrieve a workflow definition    |
| DELETE | `/workflow/{id}`                        | Delete a workflow definition      |
| POST   | `/instance/{workflowId}`                | Start a new workflow instance     |
| POST   | `/instance/{instanceId}/action/{action}`| Execute a transition              |
| GET    | `/instance/{instanceId}`                | Get current state & history       |

---

## 📦 Sample JSON Payloads

### ➕ Create Workflow

```json
{
  "id": "leave-approval",
  "states": [
    { "id": "submitted", "isInitial": true, "isFinal": false, "enabled": true },
    { "id": "approved", "isInitial": false, "isFinal": true, "enabled": true },
    { "id": "rejected", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "approve", "name": "Approve", "fromStates": ["submitted"], "toState": "approved", "enabled": true },
    { "id": "reject", "name": "Reject", "fromStates": ["submitted"], "toState": "rejected", "enabled": true }
  ]
}
```

---

## 🧪 Unit Testing

This project includes a basic unit test suite written using [xUnit](https://xunit.net/) to validate core workflow functionality.

### ✅ Covered Test Scenarios:

| Test Case | Description |
|-----------|-------------|
| `WorkflowDefinition_Should_Have_One_Initial_State` | Ensures that a workflow definition is invalid if it contains more than one initial state. |
| `Can_Transition_From_Valid_State` | Simulates a valid action execution and verifies that the workflow instance transitions to the correct state and logs the transition. |

---

### 🛠 How to Run Tests

Make sure the app is **not running** in another terminal (stop it with `Ctrl+C` if needed), then:

```bash
dotnet test
```

You should see output indicating the test results:

```bash
Passed!  - Failed: 0, Passed: 2, Skipped: 0
```

---

## 🔍 Assumptions & Design Notes

- Each workflow must contain **exactly one** `isInitial: true` state.
- Final states do not allow further transitions.
- Action transitions must originate from valid current states.
- All data is stored in-memory (resets on app restart).
- Input validation is enforced during all operations.
- The system is time-boxed (~2 hrs) and designed for clarity over completeness.

---

## 🚫 Known Limitations

- No database or persistent storage.
- No user authentication or multi-tenancy.
- No UI — backend API only.
- No support for parallel branching workflows.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 📝 Author
Mozeel Vanwani | IIT Kharagpur CSE

Email: [mozeel.vanwani@gmail.com]
