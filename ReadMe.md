# Elevator Simulator

A C# console application simulating elevator movement in a large building, built with Clean Architecture, SOLID principles, and Test-Driven Development.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [How to Run](#how-to-run)
- [How to Run Tests](#how-to-run-tests)
- [Using the Simulation](#using-the-simulation)
- [Architecture](#architecture)
- [Design Patterns & Choices](#design-patterns--design-choices)
- [Dispatching Algorithm](#dispatching-algorithm)
- [Elevator Types](#elevator-types)
- [Assumptions](#assumptions)
- [Known Limitations & Future Extensions](#known-limitations--future-extensions)

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A terminal / command prompt
- Git

---

## Setup

```bash
# Clone the repository
git clone https://github.com/Graduate-Program-26/Dotnet-Elevator-Mihir.git

# Restore dependencies
dotnet restore
```

---

## How to Run

```bash
# From the solution root
dotnet run --project src/ElevatorSim.Console/ElevatorSim.Console.csproj
```

Or navigate into the console project first:

```bash
cd src/ElevatorSim.Console
dotnet run
```

The simulation starts with **3 elevators** across **20 floors** with a capacity of **10 passengers** per elevator. These defaults can be changed in `Program.cs` via the `SimulationConfig` record.

---

## How to Run Tests

```bash
# From the solution root
dotnet test
```

To see verbose output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

All tests are isolated. No console I/O, no global state, no external dependencies. Mocks are provided via Moq.

---

## Using the Simulation

On startup the console displays a live status table of all elevators:

```
╔══════════╦═══════╦═══════════╦══════════╦═════════════╗
║ Elevator ║ Floor ║ Direction ║  State   ║  Passengers ║
╠══════════╬═══════╬═══════════╬══════════╬═════════════╣
║    #1    ║   1   ║ Stationary║   Idle   ║   0 / 10    ║
║    #2    ║   1   ║ Stationary║   Idle   ║   0 / 10    ║
║    #3    ║   1   ║ Stationary║   Idle   ║   0 / 10    ║
╚══════════╩═══════╩═══════════╩══════════╩═════════════╝

  [1] Call elevator   [2] View status   [Q] Quit
```

### Calling an elevator

Select `[1]` and follow the prompts:

```
  Enter floor number (1–20): 3
  Enter destination floor (1–20): 15
  Enter number of passengers: 4
```

The simulation will:

1. Group passengers by destination proximity
2. Determine how many elevators are needed
3. Assign each group to the most cost-efficient elevator
4. Dispatch and deliver, reporting each stop in the activity log

### Activity log

The bottom of the screen shows recent activity:

```
  Recent activity:
  ─────────────────────────────────────────
  > Elevator #1 dispatched to floor 3 — 2 passenger(s) boarding.
  > Elevator #2 dispatched to floor 3 — 2 passenger(s) boarding.
  > Elevator #1 arrived at floor 8. 1 passenger(s) dropped off.
  > Elevator #1 arrived at floor 15. 1 passenger(s) dropped off.
  > Elevator #2 arrived at floor 12. 2 passenger(s) dropped off.
```

### Input validation

All input is validated before processing. Invalid floor numbers, out-of-range values, zero passengers, or a destination matching the origin floor will display a clear error message and prompt the user to try again.

---

## Architecture

The solution follows **Clean Architecture** with dependencies pointing strictly inward.

```
ElevatorSim/
├── src/
│   ├── ElevatorSim.Domain/          # Core domain — zero external dependencies
│   │   ├── Abstractions/
│   │   │   ├── IElevator.cs
│   │   │   └── IDispatchStrategy.cs
│   │   ├── Base/
│   │   │   └── ElevatorBase.cs
│   │   ├── Models/
│   │   │   ├── ElevatorStatus.cs    # Immutable record
│   │   │   └── Passenger.cs         # Immutable record
│   │   ├── Enums/
│   │   │   ├── ElevatorDirection.cs
│   │   │   └── ElevatorState.cs
│   │   └── Exceptions/
│   │       ├── InvalidFloorException.cs
│   │       ├── CapacityExceededException.cs
│   │       └── InvalidElevatorOperationException.cs
│   │
│   ├── ElevatorSim.Application/     # Use cases and orchestration
│   │   ├── Controllers/
│   │   │   └── ElevatorController.cs
│   │   ├── ElevatorTypes/
│   │   │   ├── PassengerElevator.cs
│   │   │   ├── FreightElevator.cs
│   │   │   └── HighSpeedElevator.cs
│   │   ├── Strategies/
│   │   │   └── NearestAvailableDispatchStrategy.cs
│   │   ├── Utilities/
│   │   │   ├── PassengerDistributor.cs
│   │   │   ├── PassengerGrouper.cs
│   │   │   ├── DestinationSorter.cs
│   │   │   └── TripCostCalculator.cs
│   │   └── Queue/
│   │       └── PassengerQueue.cs
│   │
│   ├── ElevatorSim.Infrastructure/  # DI wiring, logging
│   │   ├── DependencyInjection/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Configuration/
│   │       └── SimulationConfig.cs
│   │
│   └── ElevatorSim.Console/         # Entry point and presentation
│       ├── Program.cs
│       ├── Rendering/
│       │   ├── IConsoleRenderer.cs
│       │   └── ConsoleRenderer.cs
│       └── Input/
│           └── ConsoleInputHandler.cs
│
└── tests/
    └── ElevatorSim.Tests/
        ├── DomainTests.cs
        ├── ApplicationLayerTests.cs
        └── PassengerTests.cs
```

---

## Design Patterns & Design Choices

### Strategy Pattern — Dispatching

`IDispatchStrategy` defines a contract for selecting an elevator. `NearestAvailableDispatchStrategy` is the concrete implementation. Adding a new strategy requires only a new class and no changes to `ElevatorController`. This directly satisfies the Open/Closed Principle.

### Template Method Pattern — Elevator Base

`ElevatorBase` defines the shared algorithm for movement, boarding, and dropping off passengers. Concrete elevator types (`PassengerElevator`, `FreightElevator`, `HighSpeedElevator`) inherit shared behaviour. This avoids duplication while keeping each type's behaviour encapsulated.

### Immutable Records — Domain Models

`ElevatorStatus` and `Passenger` are C# `record` types which makes them immutable by design. `ElevatorStatus` is a snapshot returned by `GetStatuses()` and never reflects live mutable state. `Passenger` is created at request time and never modified. This eliminates a class of bugs where shared mutable state causes inconsistencies.

### Dependency Injection

All dependencies are registered via `Microsoft.Extensions.DependencyInjection` in `ServiceCollectionExtensions`. No `new ConcreteService()` calls exist in business logic. This makes every component independently testable and swappable.

### Event-Driven Feedback

`ElevatorController` exposes `OnElevatorMoved` as a `Action<string>?` event. The console layer subscribes to this event to display activity messages. This keeps the controller unaware of the console, satisfying the Dependency Inversion Principle. The controller depends on nothing in the presentation layer.

### Static Utility Classes

`TripCostCalculator`, `PassengerGrouper`, `DestinationSorter`, and `PassengerDistributor` are stateless static classes. They contain pure functions with no side effects, making them trivial to unit test and reason about. They were not made into injectable services because they have no dependencies and no reason to be swapped at runtime.

### Interface Segregation

Interfaces are kept small and client-specific:

- `IElevator` — state and movement for the elevator itself
- `IDispatchStrategy` — selection logic only
- `IElevatorController` — orchestration contract for the console layer
- `IConsoleRenderer` — display contract for the input handler

No class is forced to implement methods it does not use.

---

## Dispatching Algorithm

Elevator selection uses a **minimum trip cost** algorithm rather than simple nearest-elevator dispatch.

### Trip cost formula

```
cost = (distanceToOrigin + totalStopDistance) / speed + directionChangePenalty
```

Where:

- `distanceToOrigin` — floors between elevator's current position and the pickup floor
- `totalStopDistance` — sum of distances between each destination stop in delivery order
- `speed` — elevator-specific floors per second (`PassengerElevator: 1`, `FreightElevator: 2`, `HighSpeedElevator: 3`)
- `directionChangePenalty` — a fixed penalty added per direction reversal required, applied only when the elevator is already moving

This means a `HighSpeedElevator` three floors away will be preferred over a `PassengerElevator` one floor away if the destination is far enough to make the speed advantage worthwhile.

### Passenger grouping

When multiple passengers request from the same floor, they are grouped by **destination proximity** before elevator assignment:

1. Sort passengers by destination floor
2. Find the largest gaps between consecutive destinations
3. Split at those gaps to form natural clusters (e.g. floors 2–5 form one group, floors 15–20 form another)
4. Assign each group to the cheapest elevator for that group's route

This ensures elevators are used for routes they are naturally suited to rather than all passengers piling into one elevator.

### Direction awareness

An elevator already moving in a direction incurs a penalty for reversing. This means a passing elevator heading the same way as the passengers will be preferred over a closer elevator heading the wrong way which matches real-world elevator behaviour.

### Overflow handling

If passengers exceed a single elevator's capacity, multiple elevators are selected. The number of elevators needed is determined upfront by summing available capacity until all passengers are accounted for. Remaining requests that cannot be fulfilled immediately are queued in `PassengerQueue` for retry.

---

## Elevator Types

| Type                | Capacity | Speed        | Behaviour                                                                      |
| ------------------- | -------- | ------------ | ------------------------------------------------------------------------------ |
| `PassengerElevator` | 10       | 1 floors/sec | Standard movement, stops at every requested floor                              |
| `FreightElevator`   | 20       | 2 floors/sec | Higher capacity, slower movement for heavy loads                               |
| `HighSpeedElevator` | 6        | 3 floors/sec | Lower capacity, skips intermediate floors, fastest delivery for distant floors |

All three implement `IElevator` and are fully substitutable — the dispatcher works against the interface only, never a concrete type. This satisfies the Liskov Substitution Principle: any elevator type can replace any other without changing system behaviour.

---

## Assumptions

- The building has **1 to 20 floors** by default (configurable via `SimulationConfig`)
- All elevators start at **floor 1** on initialisation
- Passenger count per request must be a **positive integer**
- A passenger's destination floor cannot be the **same as their origin floor**
- All passengers in a single request originate from the **same floor**
- Elevator doors are not simulated in the synchronous version. Boarding and deboarding are instantaneous
- The simulation runs synchronously - elevator movement completes before the next user input is accepted
- Floor numbering starts at 1 (no basement floors)
- `FreightElevator` and `HighSpeedElevator` are available in the codebase but the default simulation registers only `PassengerElevator` instances — this is changeable in `ServiceCollectionExtensions`

---

## Known Limitations & Future Extensions

### Limitations

- The simulation is synchronous — elevators do not move concurrently. In a real building multiple elevators would move simultaneously. Async/await with `Task.Delay` per floor would model this accurately.
- Passengers from different origin floors cannot be combined in a single request. Each `RequestElevator` call represents one floor's worth of waiting passengers.
- The pending request queue is checked only at dispatch time, not on a background tick — queued requests are not automatically retried when an elevator becomes free.

### Future extensions

- `async`/`await` concurrent elevator movement with per-floor delay
- Serilog structured logging to file for audit trail
- Basement floor support (negative floor numbers)
- Priority floors (e.g. ground floor always served first)
- Weight-based capacity instead of passenger count
- REST API layer replacing the console for remote control
- Persistence of simulation state across sessions
