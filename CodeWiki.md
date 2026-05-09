# Sango Infinity UI资源学习文档

## 概述

本文档详细记录了Sango Infinity项目中UI资源的结构、组件类型、层级关系以及使用规范。基于对`Assets/Mods/Content/Assets/UI`目录下的资源分析整理。

---

## 一、UI资源目录结构

```
Assets/Mods/Content/Assets/UI/
├── Animation/          # UI动画资源
├── AtlasTexture/        # 图集纹理资源
├── Font/               # 字体资源
├── Materials/          # UI材质资源
├── Prefab/             # UI预设体
│   ├── Common/         # 通用UI组件
│   ├── Field/          # 字段类UI组件
│   └── Map/            # 地图UI组件
├── Texture/            # 普通纹理资源
```

---

## 二、AtlasTexture精灵资源汇总

### 2.1 资源分类

| 分类前缀 | 数量 | 用途说明 |
|---------|------|---------|
| 4846-2  | 38张 | 人物立绘/头像系列 |
| 4846-3  | 24张 | 人物表情/状态系列 |
| 4846-4  | 128张| 场景背景/建筑系列 |
| 4846-5  | 19张 | UI装饰边框系列 |
| 4846-6  | 87张 | UI按钮/图标系列 |
| 4846-7  | 120张| 地图元素/地形系列 |
| 4846-8  | 25张 | 特效图标系列 |
| 4846-9  | 60张 | 兵种图标系列 |
| 4846-34 | 17张 | 特殊装饰系列 |
| 4849-2  | 15张 | 物品图标系列 |
| 4849-6  | 10张 | 技能图标系列 |
| 4854-1  | 34张 | 装备图标系列 |
| 4854-3  | 35张 | 城市建筑系列 |
| 4854-4  | 16张 | 科技图标系列 |
| 4854-5  | 25张 | 科技树系列 |
| 4855-1  | 1张  | 标题背景 |
| 4856-1  | 1张  | Logo资源 |
| 4858-1~3| 3张  | 特殊素材 |

### 2.2 精灵命名规范

精灵图片采用`{类别编号}_{序号}.png`格式命名，便于程序按类别加载。

---

## 三、UI Prefab组件分类

### 3.1 Common通用组件（116个）

#### 3.1.1 基础框架类

| Prefab名称 | GUID | 用途 | 组件结构 |
|-----------|------|-----|---------|
| `bg_panel` | 0c5d02b2 | 面板背景容器 | RectTransform + content/name子节点 |
| `win_frame` | eba30dda | 窗口框架 | 带l/r/t/b四边装饰的窗口边框 |
| `win_frame2~5` | - | 窗口框架变体 | 不同风格的窗口边框 |
| `panel_frame` | 52725172 | 内容面板框架 | 带装饰边的内容容器 |
| `frame_bg` | fecba18f | 框架背景图 |

#### 3.1.2 区域装饰类

| Prefab名称 | 用途 | 组件结构 |
|-----------|------|---------|
| `area_1` | 单边装饰区域 | 包含t(上)/d(下)/l(左)/r(右)四个装饰点 |
| `area_2` | 边框区域变体 | 不同边数装饰 |
| `area_3` | 三边装饰 | 三边装饰的区域 |
| `area_5` | 五边装饰 | 五边装饰区域 |
| `area_6` | 六边装饰 | 六边装饰区域 |
| `area_table` | 表格区域 | 表格样式的区域容器 |
| `area_area_area` | 复合区域 | 多重嵌套区域(lt/rd/ld/rt/r_area) |

**区域结构说明**：
```
area_area_area (根节点)
├── lt      (左上装饰)
├── rd      (右下装饰)  
├── ld      (左下装饰)
├── rt      (右上装饰)
└── r_area  (右侧区域)
    ├── rd_1
    ├── rt_1
    └── r_area_1
        ├── rd_1
        └── rt_1
```

#### 3.1.3 按钮组件类

| Prefab名称 | 用途 | 组件类型 |
|-----------|------|---------|
| `button_1` | 普通按钮 | Image + Button + lab子节点(Text) |
| `button_2` | 按钮变体2 | 不同尺寸/样式 |
| `button_3` | 按钮变体3 | 不同尺寸/样式 |
| `button_4` | 按钮变体4 | 不同尺寸/样式 |
| `button_5` | 按钮变体5 | 不同尺寸/样式 |
| `button_ok` | 确定按钮 | 绿色系确认按钮 |
| `button_cancel` | 取消按钮 | 红色/灰色取消按钮 |
| `button_all` | 全选按钮 |
| `button_deep_blue` | 深蓝色按钮 |
| `button_egg_purple` | 紫色特殊按钮 |
| `long_btn` | 长条按钮 |
| `short_btn` | 短按钮 |
| `mini_btn` | 迷你按钮 |
| `max_btn` | 最大化按钮 |
| `menu_okBtn` | 菜单确定按钮 |
| `menu_retunBtn` | 菜单返回按钮 |
| `menu_item_btn` | 菜单项按钮 |
| `sort_button_1/2` | 排序按钮 |

**按钮结构标准**：
```
button_X
├── Image (按钮背景)
└── lab   (Text子节点，显示文字)
```

#### 3.1.4 文本输入/显示类

| Prefab名称 | 用途 | 结构说明 |
|-----------|------|---------|
| `textField_1~12` | 文本字段 | Image + label(Text) + title子节点 |
| `textField_with_midIcon` | 带图标文本 | 中间带图标的文本字段 |
| `InputField` | 输入框 | 可输入文本的字段 |
| `numberField` | 数字字段 | 只允许输入数字 |
| `doubleTextField` | 双列文本 | two1/two2两个并列文本 |
| `value_field` | 数值显示字段 |
| `troop_type_lv_field` | 兵种等级字段 |

**文本字段标准结构**：
```
textField_X
├── Image
├── label    (Text组件，显示内容)
└── title    (Image子节点，背景装饰)
    └── label (标题文本)
```

#### 3.1.5 列表/容器类

| Prefab名称 | 用途 |
|-----------|------|
| `listItem` | 列表项 |
| `object_list` | 对象列表容器 |
| `selectItem` | 选择项 |
| `scenarioItem` | 剧本条目 |
| `scenarioSaveItem` | 剧本保存条目 |
| `buildingTypeItem/2` | 建筑类型项 |
| `itemType/Item/Item2/Rect/Slider` | 物品类型系列 |
| `techniqueItem/TitleItem` | 科技项目/标题 |
| `disaster_icon_list` | 灾害图标列表 |
| `modItem` | MOD项目 |

#### 3.1.6 武将/人物相关

| Prefab名称 | 用途 | 组件结构 |
|-----------|------|---------|
| `person` | 人物基础组件 | 带icon(RawImage)和name(Text) |
| `person_1` | 人物组件变体 | 带namebg和featurebg |
| `personArea` | 人物区域 | 人物显示区域容器 |
| `personArea2/3` | 人物区域变体 | 不同布局 |
| `personStatus` | 人物状态框 | 复杂嵌套结构 |
| `personStatus_big` | 大号人物状态框 |
| `personStatus_selectBtn` | 带选择的人物状态 |
| `item_person` | 人物列表项 |
| `item_select_person_btn` | 人物选择按钮 |
| `messagePersonItem` | 消息人物项 |
| `messageTextItem` | 消息文本项 |
| `forceElementItem` | 势力元素项 |

**personStatus完整结构**：
```
personStatus
├── bg (Image)
├── area_6 (嵌套区域)
│   ├── lt/rd/ld/rt (四角装饰)
│   └── r_area
│       ├── rd_1/rt_1
│       └── r_area_1
│           ├── rd_1
│           └── rt_1
├── person_1 (UIPersonItem)
│   ├── icon (RawImage，头像)
│   ├── namebg
│   │   └── name (Text)
│   └── featurebg
│       └── feature (Text，特性)
└── status_little (UIStatusItem)
    ├── hex (Image，状态背景)
    │   └── label_1~5 (Text)
    └── textField_5/7/8/11/12 (状态值)
```

#### 3.1.7 滑动条/进度条类

| Prefab名称 | 用途 |
|-----------|------|
| `slider` | 滑动条基础 |
| `slider_h` | 水平滑动条 |
| `slider_v` | 垂直滑动条 |
| `Scrollbar_h` | 水平滚动条 |
| `Scrollbar_v` | 垂直滚动条 |
| `progress` | 进度条 |
| `itemTypeSlider` | 类型滑动条 |
| `itemTypeSliderRect` | 矩形滑动条 |

#### 3.1.8 开关/切换类

| Prefab名称 | 用途 |
|-----------|------|
| `toggle_1/2/3` | 开关组件 |
| `status` | 状态显示 |
| `status_little` | 小号状态显示 |
| `energyframe` | 能量框架 |

#### 3.1.9 线条/分隔类

| Prefab名称 | 用途 |
|-----------|------|
| `line` | 分隔线 |
| `line2` | 分隔线变体 |

#### 3.1.10 特殊功能类

| Prefab名称 | 用途 | 特殊组件 |
|-----------|------|---------|
| `action_value` | 动作数值 | UITextField |
| `display_object_root` | 显示对象根节点 |
| `map` | 地图组件 |
| `worker` | 工人组件 |
| `page` | 页面组件 |
| `city_item` | 城市列表项 |
| `head` | 头像组件 |
| `menu_frame` | 菜单框架 |
| `menuItem/2/3` | 菜单项 |
| `menuSubItem1/2` | 子菜单项 |
| `techniqueArea` | 科技区域 |
| `scenario_city_map` | 剧本城市地图 |
| `playerInfoPanel` | 玩家信息面板 |
| `city_item` | 城市条目 |
| `map` | 地图显示 |

### 3.2 Field字段组件（9个）

| Prefab名称 | 用途 |
|-----------|------|
| `bigTitleField` | 大标题字段 |
| `dropdownField` | 下拉字段 |
| `floatField` | 浮点数字段 |
| `floatSliderField` | 浮点滑动字段 |
| `integerField` | 整数字段 |
| `integerSliderField` | 整数滑动字段 |
| `titleField` | 标题字段 |
| `toggleField` | 开关字段 |
| `togglegroupField` | 开关组字段 |

### 3.3 Window窗口组件

#### 3.3.1 城市相关窗口

| Prefab名称 | 用途 |
|-----------|------|
| `window_city_bar` | 城市信息栏 |
| `window_city_info_panel` | 城市信息面板 |
| `window_city_recruit` | 武将招揽窗口 |
| `window_city_recruit_troops` | 兵员招募 |
| `window_city_searching` | 探索窗口 |
| `window_city_trade` | 贸易窗口 |
| `window_city_train_troops` | 训练部队 |
| `window_city_reward` | 奖励窗口 |
| `window_city_Inspection` | 巡查窗口 |
| `window_city_person_call` | 招唤武将 |
| `window_city_person_transform` | 武将调遣 |
| `window_city_set_counsellor` | 任命太守 |
| `window_city_command_base` | 命令基础 |
| `window_city_complete` | 完成提示 |
| `window_city_create_items` | 创建物品 |
| `window_city_create_transport` | 创建运输 |
| `window_city_create_troop` | 创建部队 |
| `window_city_building_bar` | 建筑栏 |
| `window_city_building_select` | 建筑选择 |
| `window_city_building_upgrade` | 建筑升级 |
| `window_city_building_work_set` | 建筑工作设置 |
| `window_city_diplomacy_*` | 外交系列窗口 |

#### 3.3.2 情报信息窗口

| Prefab名称 | 用途 |
|-----------|------|
| `window_information_city` | 城市情报 |
| `window_information_force` | 势力情报 |
| `window_information_person` | 武将情报 |
| `window_information_troop` | 部队情报 |
| `window_information_port_gate` | 关卡情报 |
| `window_person_recruit_info` | 武将招募情报 |
| `window_player_message` | 玩家消息 |

#### 3.3.3 交互窗口

| Prefab名称 | 用途 |
|-----------|------|
| `window_dialog` | 对话窗口 |
| `window_dialog2/3/4` | 对话窗口变体 |
| `window_choice` | 选择窗口 |
| `window_contextMenu` | 右键菜单 |
| `window_contextMenu_command/system/object/other` | 分类菜单 |
| `window_object_display` | 对象显示 |
| `window_object_pop_info` | 弹出信息 |
| `window_object_selector` | 对象选择器 |
| `window_skill_crit` | 技能暴击窗口 |

#### 3.3.4 剧本/游戏窗口

| Prefab名称 | 用途 |
|-----------|------|
| `window_start` | 开始窗口 |
| `window_game` | 游戏主窗口 |
| `window_game_setting` | 游戏设置 |
| `window_loading` | 加载窗口 |
| `window_scenario_select` | 剧本选择 |
| `window_scenario_force_select` | 势力选择 |
| `window_scenario_save/load` | 存档相关 |
| `window_scenario_variables` | 剧本变量 |

#### 3.3.5 其他功能窗口

| Prefab名称 | 用途 |
|-----------|------|
| `window_technique` | 科技窗口 |
| `window_technique_complete` | 科技完成 |
| `window_troop_bar` | 部队信息栏 |
| `window_troop_build` | 部队建造 |
| `window_corps_appoint` | 军团任命 |
| `window_corps_setting` | 军团设置 |
| `window_mod_manager` | MOD管理 |
| `window_aniTextInfo` | 动画文本信息 |
| `window_mobile_cancel` | 取消确认 |

---

## 四、UI组件设计规范

### 4.1 Prefab命名规范

- **通用组件**: `类型_序号/描述` (如 `button_1`, `textField_3`)
- **窗口组件**: `window_功能模块_子功能` (如 `window_city_recruit`)
- **区域装饰**: `area_边数` (如 `area_1`, `area_6`)
- **变体组件**: `原类型_变体标识` (如 `personArea2`, `personArea3`)

### 4.2 层级结构规范

#### 4.2.1 窗口标准结构
```
window_xxx (根Canvas + UIXXX脚本)
├── mask (可选，背景遮罩)
└── root (UIKeepInScreen + DragController)
    ├── win_frame (窗口框架)
    │   ├── bg (背景图)
    │   ├── l/r/t/b (四边框)
    │   ├── titleframe (标题背景)
    │   ├── title (标题文字)
    │   ├── button_ok/cancel (按钮)
    │   └── 其他内容
    └── content (主要内容区域)
```

#### 4.2.2 面板标准结构
```
panel_name
├── panel_frame (面板框架)
│   ├── bg
│   ├── l/r/t/b (装饰边框)
│   └── title (可选)
├── top (顶部区域)
├── content (内容区域，VerticalLayoutGroup)
└── bottom (底部区域)
```

#### 4.2.3 列表项标准结构
```
listItem
├── bg (背景)
├── icon (图标)
├── label (主文字)
└── value (数值/副文字)
```

### 4.3 组件Override处理

#### 4.3.1 Override层级标记
- `isNestedRoot: true` 表示该对象是嵌套预制的根节点
- `nestingDepth` 表示嵌套层级深度
- 实例化后的对象可独立修改override属性

#### 4.3.2 常见Override场景
1. **Text内容修改**: label子节点的Text.text属性
2. **Image替换**: bg等装饰图片的Sprite
3. **组件属性**: 位置、尺寸、颜色等Transform和Image属性
4. **显示/隐藏**: 整个子对象的SetActive

### 4.4 Sango UI组件

项目中使用的自定义UI组件位于`Sango.UI`命名空间：

| 组件类名 | 用途 |
|---------|------|
| `UIPersonItem` | 人物显示组件 |
| `UIStatusItem` | 状态显示组件 |
| `UITextField` | 文本字段组件 |
| `UIDoubleTextField` | 双列文本组件 |
| `UIDialog` | 对话框组件 |
| `UICityInfoPanel` | 城市信息面板 |
| `UICityRecruit` | 城市招募 |
| `UIKeepInScreen` | 屏幕边界控制 |
| `UIKeepInScreen` | 屏幕边界控制 |
| `UISFXPlay` | 音效播放 |
| `DrawStatusComponent` | 绘制状态组件 |
| `ScaleWithScreenHeight` | 屏幕高度缩放 |
| `DragController` | 拖拽控制器 |

---

## 五、常用UI开发模式

### 5.1 创建新窗口
1. 基于现有窗口Prefab复制
2. 修改根节点名称和绑定的Handler类
3. 添加必要的Sango.UI组件
4. 配置Canvas和GraphicRaycaster

### 5.2 复用Common组件
1. 从Common目录拖拽组件到目标Prefab
2. 设置适当的Anchor和Pivot
3. Override需要的属性
4. 添加事件绑定

### 5.3 状态管理
- 使用UITextField.SetInfo()方法更新显示
- 使用UIStatusItem管理武将状态
- 通过Controller脚本处理业务逻辑

---

## 六、资源引用路径

### 6.1 Prefab路径规范
```
Assets/Mods/Content/Assets/UI/Prefab/
├── Common/{组件名}.prefab
├── Field/{组件名}.prefab
└── window_{功能}.prefab
```

### 6.2 图集路径规范
```
Assets/Mods/Content/Assets/UI/AtlasTexture/
├── {类别编号}/
│   └── {类别编号}_{序号}.png
└── {独立图片}.png
```

### 6.3 代码加载示例
```csharp
// 加载Common组件
Resources.Load<GameObject>("UI/Prefab/Common/button_1");

// 加载窗口
Resources.Load<GameObject>("UI/Prefab/window_dialog");

// 加载图集
Resources.Load<Texture2D>("UI/AtlasTexture/4846-2/4846-2_0");
```

---

## 附录：组件清单

### A.1 按钮类完整列表
- button_1~5, button_ok, button_cancel, button_all
- button_deep_blue, button_egg_purple
- long_btn, short_btn, mini_btn, max_btn
- menu_okBtn, menu_retunBtn, menu_item_btn
- sort_button_1, sort_button_2
- item_select_person_btn

### A.2 文本字段完整列表
- textField_1~12, textField_with_midIcon
- InputField, numberField, value_field
- doubleTextField, troop_type_lv_field
- bigTitleField, titleField

### A.3 区域装饰完整列表
- area_1, area_2, area_3, area_5, area_6
- area_area_area, area_table

### A.4 人物相关完整列表
- person, person_1
- personArea, personArea2, personArea3
- personStatus, personStatus_big, personStatus_selectBtn
- item_person, messagePersonItem, messageTextItem

---

*文档生成时间: 2026-05-08*
*项目版本: Sango Infinity*
