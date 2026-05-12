---
name: "code-comment-optimizer"
description: "Optimizes and completes code comments for C# projects. Invoke when user requests code comment optimization or completion for their project."
---

# Code Comment Optimizer

This skill helps optimize and complete code comments for C# projects, ensuring clear, consistent, and comprehensive documentation.

## When to Use
- When user requests code comment optimization for their project
- When user asks to complete missing code comments
- When reviewing code for documentation quality
- When preparing code for team collaboration or open source release

## How to Use

### 1. Analyze the Codebase
- Identify files and classes that need comment optimization
- Determine the current comment style and consistency
- Prioritize files based on their importance and complexity

### 2. Add Class Comments
- Add XML-style comments for each class
- Include a brief description of the class purpose
- Mention key responsibilities and usage

### 3. Add Property Comments
- Add XML-style comments for each property
- Describe the purpose and usage of each property
- Include any relevant constraints or relationships

### 4. Add Method Comments
- Add XML-style comments for each method
- Include method purpose and functionality
- Document all parameters with `param` tags
- Document return values with `returns` tags
- Include any exceptions with `exception` tags if applicable

### 5. Ensure Consistency
- Use consistent comment format across all files
- Follow the same structure for similar elements
- Maintain consistent terminology and style

### 6. Verify Completeness
- Check that all public and protected members are documented
- Ensure complex logic is explained
- Verify that comments are up-to-date with code changes

## Best Practices

### Comment Format
Use XML-style comments for C# code:

```csharp
/// <summary>
/// Brief description of the class, property, or method
/// </summary>
/// <param name="paramName">Description of the parameter</param>
/// <returns>Description of the return value</returns>
```

### Comment Content
- Be concise but informative
- Focus on why the code does something, not just what it does
- Avoid redundant comments that repeat the code
- Use proper grammar and punctuation

### Special Cases
- For interfaces: Document the intended behavior and usage
- For abstract classes: Document the expected implementation
- For static classes: Document the utility or helper functions

## Example

**Before:**
```csharp
public class Map
{
    public int Width { get; internal set; }
    public int Height { get; internal set; }
    
    public void Load(Scenario scenario)
    {
        // Load map data
    }
}
```

**After:**
```csharp
/// <summary>
/// 地图类，管理游戏地图的加载、创建和路径查找等功能
/// </summary>
public class Map
{
    /// <summary>
    /// 地图宽度
    /// </summary>
    public int Width { get; internal set; }
    
    /// <summary>
    /// 地图高度
    /// </summary>
    public int Height { get; internal set; }
    
    /// <summary>
    /// 加载地图
    /// </summary>
    /// <param name="scenario">场景对象</param>
    public void Load(Scenario scenario)
    {
        // Load map data
    }
}
```

## Common Issues to Fix
- Missing class or method comments
- Incomplete parameter or return value documentation
- Inconsistent comment formatting
- Outdated comments that don't match code changes
- Lack of context for complex logic

This skill helps ensure that code is well-documented, making it easier to understand, maintain, and extend.