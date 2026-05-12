---
name: "game-message-system"
description: "Manages in-game messages for city capture, character transfer, and prisoner actions. Invoke when implementing or modifying game message functionality."
---

# Game Message System

This skill provides a comprehensive guide to implementing and managing in-game messages for various game events, including city capture, character transfer, and prisoner actions.

## Overview

The game message system is responsible for displaying text messages to the player when specific game events occur. It uses an event-driven architecture to listen for game events and generate appropriate messages based on the context.

## Key Features

### 1. City Capture Messages

- **Attackers are current player**: "我军成功攻陷了[城市名]！"
- **Defenders are current player**: "我方[城市名]被[进攻方势力名]攻陷了！"
- **Neutral scenario**: "[城市名]被[进攻方势力名]攻陷了！"

### 2. Character Transfer Messages

- **Successful transfer**: "[武将名]已成功从[来源城市]转移到[目标城市]！"

### 3. Prisoner Action Messages

- **Escape**: 
  - Prisoner is current player: "[武将名]成功逃脱了！"
  - Captor is current player: "我方关押的[武将名]逃跑了！"

- **Release**: 
  - Prisoner is current player: "[武将名]被[释放方势力名]释放了！"
  - Releaser is current player: "我方释放了[武将名]！"

- **Execution**: 
  - Prisoner is current player: "[武将名]被[斩杀方势力名]斩杀了！"
  - Executioner is current player: "我方斩杀了[武将名]！"

## Implementation

### Event System

The message system uses the `GameEvent` class to listen for game events:

- `OnCityFall`: Triggered when a city is captured
- `OnPersonChangeCityComplete`: Triggered when a character successfully transfers between cities
- `OnPersonEscape`: Triggered when a prisoner escapes
- `OnPersonRelease`: Triggered when a prisoner is released
- `OnPersonExecute`: Triggered when a prisoner is executed

### Message Handling

The `PlayerMessage` class handles message creation and display:

- `_AddTextMessage`: Creates and adds text messages to the message list
- `_AddPersonMessage`: Creates and adds character messages to the message list
- Event handlers for each game event to generate appropriate messages

### Escape Types

The `EscapeType` enum defines different ways a prisoner can escape:

- `None`: Default value
- `Escape`: Prisoner escapes on their own
- `Released`: Prisoner is released by captor
- `TroopDestroyed`: Prisoner escapes when their troop is destroyed

## Usage

1. **Register event listeners** in the `Init` method of `PlayerMessage`
2. **Implement event handlers** for each game event
3. **Generate appropriate messages** based on the event context
4. **Trigger events** at the appropriate points in the game logic
5. **Clean up event listeners** in the `Clear` method

## Example Code

### Adding a new message type

```csharp
// 1. Add event in GameEvent.cs
public static EventDelegate<EventParameters> OnNewEvent;

// 2. Register listener in PlayerMessage.Init
GameEvent.OnNewEvent += OnNewEvent;

// 3. Implement event handler
private void OnNewEvent(EventParameters parameters)
{
    if (/* conditions */)
    {
        string message = "[Message content]";
        _AddTextMessage(message, force, x, y);
    }
}

// 4. Clean up in PlayerMessage.Clear
GameEvent.OnNewEvent -= OnNewEvent;

// 5. Trigger event in game logic
GameEvent.OnNewEvent?.Invoke(parameters);
```

## Best Practices

- **Use color tags** for names to improve readability: `$"{person.ColorName}已成功逃脱！"`
- **Check if the force is current player** using `IsCurPlayer` property
- **Provide context** in messages (e.g., source and target cities for transfers)
- **Handle different scenarios** (e.g., attacker vs defender for city capture)
- **Clean up event listeners** to avoid memory leaks

## Files Modified

- `GameEvent.cs`: Added event definitions
- `PlayerMessage.cs`: Added event listeners and handlers
- `Person.cs`: Added `EscapeType` enum and modified `Escape` method
- `PersonRecruit.cs`: Added event triggers for release and execution
- `City.cs`: Modified `OnPersonReturnCity` and `OnPersonTransformEnd` methods
- `Troop.cs`: Updated `Escape` method calls with `EscapeType` parameter
- `DiplomacyManager.cs`: Updated `Escape` method calls with `EscapeType` parameter
- `ForceAI.cs`: Updated `Escape` method calls with `EscapeType` parameter

This skill provides a framework for implementing and expanding the game message system, ensuring consistent and informative feedback to players for various game events.