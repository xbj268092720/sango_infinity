using System.Collections.Generic;
using UnityEngine;

namespace SangoTools.Editor
{
    /// <summary>
    /// 编辑器顶部菜单栏配置
    /// 用于定义菜单项和布局
    /// </summary>
    [CreateAssetMenu(fileName = "EditorTopMenuConfig", menuName = "SangoTools/Editor Top Menu Config")]
    public class EditorTopMenuConfig : ScriptableObject
    {
        [Header("菜单类别")]
        public MenuCategory[] menuCategories;

        [Header("布局按钮文本")]
        public string[] layoutButtonTexts = new string[] { "默认", "宽屏", "垂直" };

        [Header("默认布局索引")]
        public int defaultLayoutIndex = 0;

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static EditorTopMenuConfig CreateDefaultConfig()
        {
            var config = CreateInstance<EditorTopMenuConfig>();
            
            // 文件菜单
            var fileMenuItems = new List<MenuItemData>
            {
                new MenuItemData { displayName = "新建地图", actionId = "File_NewMap", shortcut = "Ctrl+N" },
                new MenuItemData { displayName = "打开地图", actionId = "File_OpenMap", shortcut = "Ctrl+O" },
                new MenuItemData { displayName = "保存地图", actionId = "File_SaveMap", shortcut = "Ctrl+S" },
                new MenuItemData { displayName = "另存为...", actionId = "File_SaveAs", shortcut = "Ctrl+Shift+S" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "从剧本加载", actionId = "File_LoadFromScenario" },
                new MenuItemData { displayName = "导出剧本", actionId = "File_ExportScenario" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "设置", actionId = "File_Settings" },
                new MenuItemData { displayName = "退出", actionId = "File_Exit" }
            };
            
            // 编辑菜单
            var editMenuItems = new List<MenuItemData>
            {
                new MenuItemData { displayName = "撤销", actionId = "Edit_Undo", shortcut = "Ctrl+Z" },
                new MenuItemData { displayName = "重做", actionId = "Edit_Redo", shortcut = "Ctrl+Y" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "历史记录", actionId = "Edit_History", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "清空历史", actionId = "Edit_ClearHistory" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "加载剧本", actionId = "Edit_LoadScenario" },
                new MenuItemData { displayName = "重置编辑器", actionId = "Edit_Reset" }
            };
            
            // 视图菜单
            var viewMenuItems = new List<MenuItemData>
            {
                new MenuItemData { displayName = "固定视角", actionId = "View_FixedCamera", isToggle = true, isToggleOn = false },
                new MenuItemData { displayName = "重置相机", actionId = "View_ResetCamera", shortcut = "F" },
                new MenuItemData { displayName = "适应窗口", actionId = "View_FitToWindow", shortcut = "Shift+F" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "显示网格", actionId = "View_ShowGrid", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "显示地形", actionId = "View_ShowTerrain", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "显示建筑", actionId = "View_ShowBuildings", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "显示单位", actionId = "View_ShowUnits", isToggle = true, isToggleOn = true },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "秋季", actionId = "View_Season_Autumn" },
                new MenuItemData { displayName = "春季", actionId = "View_Season_Spring" },
                new MenuItemData { displayName = "夏季", actionId = "View_Season_Summer" },
                new MenuItemData { displayName = "冬季", actionId = "View_Season_Winter" }
            };
            
            // 渲染菜单
            var renderMenuItems = new List<MenuItemData>
            {
                new MenuItemData { displayName = "灯光设置", actionId = "Render_Lighting" },
                new MenuItemData { displayName = "雾效设置", actionId = "Render_Fog" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "启用雾效", actionId = "Render_FogEnabled", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "阴影开关", actionId = "Render_Shadows", isToggle = true, isToggleOn = true },
                new MenuItemData { displayName = "抗锯齿", actionId = "Render_AntiAliasing", isToggle = true, isToggleOn = true }
            };
            
            // 帮助菜单
            var helpMenuItems = new List<MenuItemData>
            {
                new MenuItemData { displayName = "快捷键", actionId = "Help_Shortcuts" },
                new MenuItemData { displayName = "使用教程", actionId = "Help_Tutorial" },
                new MenuItemData { isSeparator = true },
                new MenuItemData { displayName = "检查更新", actionId = "Help_CheckUpdate" },
                new MenuItemData { displayName = "关于", actionId = "Help_About" }
            };
            
            config.menuCategories = new MenuCategory[]
            {
                new MenuCategory { categoryName = "文件", menuItems = fileMenuItems },
                new MenuCategory { categoryName = "编辑", menuItems = editMenuItems },
                new MenuCategory { categoryName = "视图", menuItems = viewMenuItems },
                new MenuCategory { categoryName = "渲染", menuItems = renderMenuItems },
                new MenuCategory { categoryName = "帮助", menuItems = helpMenuItems }
            };

            return config;
        }
    }

    /// <summary>
    /// Unity编辑器扩展：自动创建配置
    /// </summary>
#if UNITY_EDITOR
    namespace EditorScripts
    {
        using UnityEditor;
        
        [CustomEditor(typeof(EditorTopMenuConfig))]
        public class EditorTopMenuConfigEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("重置为默认配置", GUILayout.Height(30)))
                {
                    var config = (EditorTopMenuConfig)target;
                    var defaultConfig = EditorTopMenuConfig.CreateDefaultConfig();
                    EditorUtility.CopySerialized(defaultConfig, config);
                    EditorUtility.SetDirty(config);
                }
            }
        }
    }
#endif
}
