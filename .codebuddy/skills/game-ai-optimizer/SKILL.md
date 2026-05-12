---
name: "game-ai-optimizer"
description: "Optimizes game AI for strategy games, including troop movement, city defense evaluation, and decision-making. Invoke when user needs to improve AI behavior or fix AI-related issues."
---

# Game AI Optimizer

This skill provides techniques and best practices for optimizing AI behavior in strategy games, based on practical experience with the Sango Infinity codebase.

## Key Optimizations

### 1. Troop AI Improvements
- **Safe Movement**: Implemented `MoveToTargetSafely` method that evaluates cell safety and terrain bonuses
- **Pathfinding**: Used `GetDirectPath` for reliable pathfinding instead of non-existent methods
- **Range Calculation**: Ensured `tempMoveRange` is properly populated before movement decisions

### 2. City Defense Evaluation
- **Balanced Calculation**: Reduced city defense strength weights to prevent部队 from retreating prematurely
- **Strategic Decision-making**: Removed overly strict defense-based retreat conditions
- **Simplified Logic**: Streamlined AI decision process to focus on enemy presence rather than static defense values

### 3. Force AI Enhancements
- **Personality System**: Added strategy-influencing fields to `Personality` class for AI behavior customization
- **Technology Research**: Fixed AI tech research logic with proper cost calculation and availability checks
- **Captive Management**: Implemented personality-based tendencies for prisoner handling

### 4. City AI Optimizations
- **Construction Logic**: Reduced minimum officer requirement from 3 to 1
- **Dynamic Planning**: Updated construction logic to check `freePersons.Count` instead of assuming fixed values
- **Resource Management**: Restored construction count limits for better resource allocation

## Code Structure Best Practices
- **Utility Methods**: Centralized AI utility functions in `TroopAIUtility` for better code organization
- **Type Safety**: Fixed type-related issues and ensured proper null checks
- **Performance**: Optimized pathfinding and movement calculations for better runtime performance

## Troubleshooting Tips
- **Debugging**: Use `GetDiagnostics` to identify compilation errors
- **Testing**: Verify AI behavior by observing troop movements and decision-making
- **Balancing**: Adjust weight values and thresholds to achieve desired AI behavior

## Usage Examples
- When troops are retreating too early: Check defense strength calculations and adjust weights
- When AI gets stuck: Ensure proper pathfinding and movement range calculation
- When AI makes poor decisions: Review personality-based behavior and adjust parameters