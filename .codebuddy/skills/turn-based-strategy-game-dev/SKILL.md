---
name: "turn-based-strategy-game-dev"
description: "Provides knowledge and implementation patterns for turn-based strategy games, including diplomacy systems, prisoner escape mechanics, skill frameworks, and UI parameter management. Invoke when developing or enhancing turn-based strategy game features."
---

# Turn-Based Strategy Game Development

This skill provides comprehensive knowledge and implementation patterns for developing turn-based strategy games, based on the Romance of the Three Kingdoms 11-inspired project.

## Key Systems and Implementations

### Diplomacy System

**Core Components:**
- `DiplomacyManager`: Manages force relationships and diplomatic actions
- `DiplomacyEventManager`: Handles random diplomatic event triggers
- `DiplomacySystem`: Coordinates diplomatic interactions

**Key Features:**
- Alliance, truce, declare war, and other diplomatic actions
- Diplomatic missions requiring generals to travel to target cities
- 5 new diplomatic events: technical exchange, military aid request, marriage proposal, territorial dispute, and common enemy
- Relationship management using `RelationMap` in `Scenario`

**Implementation Pattern:**
```csharp
// Example: Performing a diplomacy action
public bool PerformDiplomacyAction(DiplomacyActionType actionType, Force sender, Force receiver, Person diplomat, object param = null)
{
    // Check if action can be performed
    // Create diplomatic mission for general
    // Set mission parameters and target city
    // Calculate travel time
}
```

### Prisoner Escape System

**Implementation:**
- `HandlePrisonerEscape()` method in `Scenario` class
- Executes at the end of each turn
- Calculates escape probability based on location (troop or city)
- Triggers `GameEvent.OnPersonEscape` event

**Key Code:**
```csharp
private void HandlePrisonerEscape()
{
    // Iterate through all prisoners
    // Calculate escape chance (max 30%)
    // Check if escape succeeds
    // Execute escape logic and trigger event
}
```

### Skill Framework

**Components:**
- `SkillInstance`: Represents an active skill
- `SkillSuccessMethod`: Handles skill success rate calculation
- `SkillCriticalMethod`: Manages skill critical hit calculation

**Features:**
- Skill performance architecture
- Movement skills and area skills
- Hexagonal grid-based skill targeting
- Critical hit system with ability-based bonuses

### UI Parameter Management

**Components:**
- `UIScenarioVariables`: UI interface for scenario variables
- `ScenarioVariables`: Core game parameters

**Features:**
- Organized parameter categories (difficulty, battle, troops, skills, etc.)
- Real-time parameter adjustment
- Validation and range controls

## Common Patterns and Best Practices

### Singleton Pattern
Used for managers like `DiplomacyManager` and `DiplomacyEventManager` to ensure global access.

### Event-Driven Design
Utilize `GameEvent` system for decoupled event handling, e.g., `OnPersonEscape` for prisoner escape events.

### Mission System
Extend `MissionType` enumeration to support new mission types like `PersonDiplomacy` for diplomatic missions.

### Turn-Based Logic
Implement turn-based mechanics in `Scenario.TurnStart()` and `Scenario.TurnEnd()` methods, integrating systems like diplomacy event checks and prisoner escape.

## Troubleshooting

### Common Issues:
- Component addition failures: Refresh Unity asset database
- Animation curve type errors: Use proper property paths
- Prefab script reference loss: Re-edit prefab files
- Namespace issues: Ensure proper using statements

### Debugging Tips:
- Use `Sango.Log.Print()` for event logging
- Validate parameter ranges in UI fields
- Test edge cases for diplomatic actions and prisoner escape

## Integration Checklist

- [ ] Ensure proper namespace usage (System.IO for IO operations)
- [ ] Avoid hard-coding values, use `ScenarioVariables` instead
- [ ] Expose user-modifiable parameters in `UIScenarioVariables`
- [ ] Implement proper error handling and validation
- [ ] Test all systems with various scenario configurations

This skill provides a comprehensive framework for developing turn-based strategy games, with a focus on modular design and extensible systems.