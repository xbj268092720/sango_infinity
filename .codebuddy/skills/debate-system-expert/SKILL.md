---
name: "debate-system-expert"
description: "Provides expertise on the debate system implementation for the Sango Infinity project. Invoke when user needs information about the debate system structure, components, or implementation details."
---

# Debate System Expert

## Overview

This skill provides detailed information about the debate system implemented in the Sango Infinity project, inspired by the debate mechanics from Romance of the Three Kingdoms 11.

## System Structure

The debate system consists of the following key components:

### Core Components

1. **DebateManager** (`Sango.Game.Debate.DebateManager`)
   - Inherits from `Sango.Singleton<DebateManager>`
   - Manages the overall debate process
   - Handles debate initialization and updates
   - Creates and manages debate instances

2. **DebateInstance** (`Sango.Game.Debate.DebateInstance`)
   - Manages a single debate between two participants
   - Handles turn management and skill usage
   - Implements victory conditions
   - Triggers debate events

3. **DebateParticipant** (`Sango.Game.Debate.DebateParticipant`)
   - Represents a participant in the debate
   - Manages morale, anger, and skills
   - Implements skill usage and refill logic

4. **DebateSkill** (`Sango.Game.Debate.DebateSkill`)
   - Represents a skill card used in debates
   - Defines skill types, levels, and effects
   - Implements skill execution logic

### UI Components

1. **DebateWindow** (`Sango.UI.Debate.DebateWindow`)
   - Inherits from `Sango.UGUIWindow`
   - Displays the debate interface
   - Shows participant information and skill cards
   - Handles user input for skill selection

2. **SkillCardUI** (`Sango.UI.Debate.SkillCardUI`)
   - Represents individual skill cards in the UI
   - Handles skill card display and interaction

### Integration

- **DebateIntegration** (`Sango.Game.Debate.DebateIntegration`)
  - Integrates the debate system into the game flow
  - Provides methods to trigger debates between characters
  - Converts game characters to debate participants

## Key Features

- **Turn-based combat system** with rounds and turns
- **Skill card system** with different types and levels
- **Morale and anger mechanics** affecting debate outcomes
- **AI decision-making** for computer-controlled opponents
- **Victory conditions** based on morale depletion or time limits
- **Event-driven architecture** with callbacks for UI updates

## Usage

1. **Initialization**: Call `DebateManager.Instance.Init()` to initialize the debate system
2. **Starting a Debate**: Use `DebateManager.Instance.StartDebate(participant1, participant2)`
3. **Updating**: Call `DebateManager.Instance.Update(deltaTime)` in the game loop
4. **Skill Usage**: Players select skills via the UI, AI uses skills automatically
5. **Ending**: The debate ends when one participant's morale reaches zero or after 50 rounds

## Integration with Game System

The debate system is integrated into the main game via:

1. **Game.cs**: Adds debate system initialization and update calls
2. **DebateIntegration.cs**: Provides methods to trigger debates between game characters

## Troubleshooting

- **DebateManager not found**: Ensure the manager is properly initialized and inherits from Singleton
- **UI not displaying**: Check that DebateWindow inherits from UGUIWindow and is properly instantiated
- **AI not acting**: Verify the AI decision logic in DebateInstance.StartTurn
- **Skills not working**: Check the skill execution logic in DebateSkill.Execute

## Example Usage

```csharp
// Initialize the debate system
DebateManager.Instance.Init();

// Create participants
DebateParticipant player = new DebateParticipant("Player", ParticipantType.Player, 100, 100);
DebateParticipant ai = new DebateParticipant("AI", ParticipantType.AI, 100, 100);

// Start a debate
DebateManager.Instance.StartDebate(player, ai);

// Update in game loop
void Update()
{
    DebateManager.Instance.Update(Time.deltaTime);
}
```