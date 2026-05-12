---
name: "game-skill-system"
description: "Provides knowledge and implementation guidance for turn-based strategy game skill systems. Invoke when working on skill-related features, debugging skill issues, or optimizing skill performance."
---

# Game Skill System

This skill provides comprehensive knowledge about implementing skill systems for turn-based strategy games, based on the Sango Infinity project.

## Key Components

### Skill Core Classes
- **Skill**: Defines skill properties and configurations
- **SkillInstance**: Handles skill execution and state
- **SkillTimeline**: Manages skill performance timing and events
- **SkillTimelineInstance**: Processes timeline events and time control
- **SkillTimelineEvent**: Base class for different skill event types

### Event Types
- PlayAnimation: Plays character animations
- PlayEffect: Triggers visual effects
- PlaySound: Plays sound effects
- ExecuteDamage: Calculates and applies damage
- ExecuteOffset: Handles unit movement/displacement
- ExecuteEffect: Applies skill effects (buffs, debuffs)
- ShowText: Displays skill text messages
- CameraShake: Triggers camera shake effects

### Range Calculations
- Ring: Circular area around target
- DirectionLine: Linear area in a specific direction
- Spiral: Expanding spiral pattern
- Fan: Cone-shaped area
- Rectangle: Rectangular area
- Cross: Cross-shaped area
- Square: Square area
- Diamond: Diamond-shaped area

### BUFF System
- Poison: Deals damage over time
- Burn: Deals fire damage over time
- Freeze: Immobilizes units
- Silence: Prevents skill usage
- Invincible: Grants temporary invulnerability
- Shield: Absorbs incoming damage

## Implementation Guidelines

### Skill Configuration
- Use JSON format for skill definitions
- Support MOD system for extensibility
- Include properties for damage, range, cost, and effects

### Timeline System
- Define events with specific timestamps
- Use inheritance for event type specialization
- Separate configuration (SkillTimeline) from execution (SkillTimelineInstance)

### Performance Optimization
- Reuse timeline instances to avoid frequent initialization
- Use efficient data structures for skill lookups
- Implement proper event sorting for timeline processing

### Testing
- Create comprehensive test systems for skill functionality
- Test different skill combinations and edge cases
- Verify skill balance and performance

## Common Issues and Solutions

### Skill Execution Problems
- Check timeline event order and timing
- Verify skill effect initialization
- Ensure proper target selection logic

### Performance Issues
- Optimize timeline event processing
- Reduce redundant calculations
- Use object pooling for frequently created objects

### MOD Integration
- Ensure proper loading of MOD skill configurations
- Handle conflicts between base and MOD skills
- Maintain backward compatibility

## Usage Examples

### Creating a New Skill
1. Define skill properties in JSON
2. Implement any custom effects
3. Configure timeline events for skill performance
4. Test skill functionality and balance

### Debugging Skill Issues
1. Check skill configuration for errors
2. Verify timeline event execution
3. Test skill under different conditions
4. Use logging to identify problem areas

### Optimizing Skill Performance
1. Profile skill execution time
2. Identify bottlenecks in timeline processing
3. Implement caching for frequently used data
4. Optimize event processing order