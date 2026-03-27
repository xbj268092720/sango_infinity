# 代码注释标准

## 1. 总则
- 所有代码文件都应包含适当的注释，提高代码可读性和可维护性
- 注释应简洁明了，避免冗余和重复
- 注释应使用中文，与代码风格保持一致
- 注释应与代码同步更新，确保准确性

## 2. 文件头部注释
每个代码文件的头部应包含以下信息：
```csharp
/*
 * 文件名：FileName.cs
 * 描述：文件功能的简要描述
 * 作者：作者名称
 * 创建日期：YYYY-MM-DD
 * 最后修改：YYYY-MM-DD
 */
```

## 3. 类注释
每个类应包含以下注释：
```csharp
/// <summary>
/// 类的功能描述
/// </summary>
public class ClassName : BaseClass
{
    // 类内容
}
```

## 4. 方法注释
每个方法应包含以下注释：
```csharp
/// <summary>
/// 方法的功能描述
/// </summary>
/// <param name="param1">参数1的说明</param>
/// <param name="param2">参数2的说明</param>
/// <returns>返回值的说明</returns>
public ReturnType MethodName(ParameterType param1, ParameterType param2)
{
    // 方法内容
}
```

## 5. 字段注释
每个字段应包含以下注释：
```csharp
/// <summary>
/// 字段的描述
/// </summary>
private int fieldName;
```

## 6. 属性注释
每个属性应包含以下注释：
```csharp
/// <summary>
/// 属性的描述
/// </summary>
public string PropertyName { get; set; }
```

## 7. 特殊注释
- `// TODO:` 标记待完成的任务
- `// FIXME:` 标记需要修复的问题
- `// NOTE:` 标记重要的说明

## 8. 注释示例

### 类注释示例
```csharp
/// <summary>
/// 相机移动事件类，用于处理相机移动和对话框显示
/// </summary>
public class CameraMoveEvent : RenderEventBase
{
    // 类内容
}
```

### 方法注释示例
```csharp
/// <summary>
/// 进入事件处理
/// </summary>
/// <param name="scenario">场景实例</param>
public override void Enter(Scenario scenario)
{
    // 方法内容
}
```

### 字段注释示例
```csharp
/// <summary>
/// 目标位置
/// </summary>
public Vector3 targetPosition;

/// <summary>
/// 移动持续时间
/// </summary>
public float moveDuration = 0.5f;
```

## 9. 注释规范
- 注释应使用完整的句子，首字母大写
- 注释应避免使用缩写，除非是广泛认可的缩写
- 注释应解释代码的"为什么"，而不是"是什么"
- 复杂的算法或逻辑应提供详细的注释
- 公共API应提供更详细的注释

## 10. 工具支持
- 使用Visual Studio的XML注释功能
- 利用IDE的自动生成注释功能
- 定期检查和更新注释
