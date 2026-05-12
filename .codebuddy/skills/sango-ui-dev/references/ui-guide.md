# Sango Infinity UI快速参考指南

## 目录结构

```
Assets/Mods/Content/Assets/UI/
├── Prefab/
│   ├── Common/           # 通用组件(116个)
│   │   ├── button_*.prefab
│   │   ├── textField_*.prefab
│   │   ├── area_*.prefab
│   │   ├── person*.prefab
│   │   └── win_frame.prefab
│   ├── Field/            # 字段组件(9个)
│   └── window_*.prefab   # 窗口预设
└── AtlasTexture/
    ├── 4846-2/          # 人物立绘(38张)
    ├── 4846-3/          # 人物表情(24张)
    ├── 4846-4/          # 场景建筑(128张)
    ├── 4846-5/          # UI边框(19张)
    ├── 4846-6/          # 按钮图标(87张)
    ├── 4846-7/          # 地图地形(120张)
    ├── 4846-8/          # 特效(25张)
    └── 4846-9/          # 兵种(60张)
```

## 常用组件速查

### 按钮类
| 名称 | 用途 |
|------|------|
| button_1~5 | 普通按钮 |
| button_ok | 确定 |
| button_cancel | 取消 |
| long_btn/short_btn | 长/短按钮 |

### 面板类
| 名称 | 用途 |
|------|------|
| bg_panel | 面板背景 |
| win_frame | 窗口框架 |
| panel_frame | 内容框架 |

### 区域装饰
| 名称 | 结构 |
|------|------|
| area_1 | 单边装饰(t/d/l/r) |
| area_6 | 六边装饰 |
| area_area_area | 复合区域 |

### 文本字段
| 名称 | 用途 |
|------|------|
| textField_1~12 | 单行文本 |
| doubleTextField | 双列文本 |
| numberField | 数字 |

### 人物组件
| 名称 | 用途 |
|------|------|
| personStatus | 人物状态框 |
| personArea | 人物区域 |
| person_1 | 人物图标 |

## 窗口列表

### 城市相关
- window_city_bar / info_panel / recruit
- window_city_trade / search / reward
- window_city_diplomacy_*

### 情报相关
- window_information_city / force / person / troop

### 剧本相关
- window_scenario_select / force_select / save

### 其他
- window_dialog / choice / contextMenu
- window_technique / start / game

## MCP工具使用

```csharp
// 搜索Prefabs
manage_asset(action="search", path="Assets/Mods/Content/Assets/UI", 
             filter_type="Prefab", page_size=100)

// 获取层级
manage_prefabs(action="get_hierarchy", 
               prefab_path="Assets/Mods/Content/Assets/UI/Prefab/Common/button_1.prefab")

// 获取详情
manage_prefabs(action="get_info", 
               prefab_path="Assets/Mods/Content/Assets/UI/Prefab/window_dialog.prefab")
```

## 自定义UI组件

Sango.UI命名空间组件：
- UIPersonItem - 人物显示
- UIStatusItem - 状态显示  
- UITextField - 文本字段
- UIDialog - 对话框
- UIKeepInScreen - 屏幕边界
- DragController - 拖拽控制
