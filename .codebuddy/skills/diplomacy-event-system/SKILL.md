---
name: "diplomacy-event-system"
description: "Manages diplomacy events using JSON configuration and mod integration. Invoke when working on diplomacy event system changes, JSON configuration, or mod-related features."
---

# Diplomacy Event System

This skill provides knowledge and functionality for the diplomacy event system in Sango Infinity, including JSON configuration and mod integration.

## Key Features

### JSON Configuration
- **Default Event Path**: `Build/Content/Data/DiplomacyEvent`
- **Auto-generation**: Creates default events if directory doesn't exist
- **Event Structure**: Includes ID, name, description, relation requirements, probability, and effects
- **Effect Types**: Support for various event effects like relation changes, trade, alliance requests, etc.

### Mod Integration
- **Mod Path**: `Data/DiplomacyEvent` within each mod
- **Loading Process**: Loads events from all active mods
- **Compatibility**: Ensures seamless integration with mod system

### Event Management
- **Event Triggering**: Limited to 3 events per quarter
- **Season Reset**: Counter resets on season update
- **Probability System**: Based on relation levels and event probability

## Implementation Details

### Event Structure
```csharp
[JsonObject(MemberSerialization.OptIn)]
public class DiplomacyEvent
{
    [JsonProperty] public int Id { get; set; }
    [JsonProperty] public string Name { get; set; }
    [JsonProperty] public string Description { get; set; }
    [JsonProperty] public int MinRelation { get; set; }
    [JsonProperty] public int MaxRelation { get; set; }
    [JsonProperty] public int Probability { get; set; }
    [JsonProperty] public string EffectType { get; set; }
    [JsonProperty] public Dictionary<string, object> EffectParams { get; set; }
    [JsonIgnore] public System.Action<Force, Force> Effect { get; set; }
}
```

### Loading Events
```csharp
private void LoadDiplomacyEvents()
{
    // Load main game events
    string mainPath = Path.Combine(Application.streamingAssetsPath, "Build", "Content", "Data", "DiplomacyEvent");
    LoadDiplomacyEventsFromPath(mainPath);

    // Load mod events
    LoadModDiplomacyEvents();
}

private void LoadModDiplomacyEvents()
{
    foreach (var mod in ModManager.Instance.GetEnabledMods())
    {
        string modPath = Path.Combine(mod.ModDir, "Data", "DiplomacyEvent");
        if (Directory.Exists(modPath))
        {
            LoadDiplomacyEventsFromPath(modPath);
        }
    }
}
```

### Effect Types
- `AddRelation`: Increases diplomatic relations
- `ReduceRelation`: Decreases diplomatic relations
- `Trade`: Initiates trade negotiations
- `AllianceRequest`: Requests alliance
- `TruceRequest`: Requests truce
- `TechniqueExchange`: Exchanges technologies
- `RequestTroops`: Requests military assistance
- `Marriage`: Proposes marriage alliance
- `CulturalExchange`: Increases relations and may grant technology
- `CommonCelebration`: Increases relations and gives gold to both parties
- `BorderTrade`: Increases relations and gives gold to both parties
- `TechnicalAssistance`: Increases relations and may grant technology
- `EconomicAssistance`: Increases relations and provides economic aid

## Usage Guidelines

1. **Creating New Events**: Add JSON files to the appropriate directory
2. **Mod Events**: Place events in `Data/DiplomacyEvent` within your mod
3. **Testing**: Verify events trigger correctly based on relation levels
4. **Debugging**: Check console logs for event loading and triggering information

## Best Practices

- Use descriptive event names and descriptions
- Balance event probabilities to ensure varied gameplay
- Test events across different relation ranges
- Ensure mod events don't conflict with base game events
- Follow the existing JSON structure for consistency