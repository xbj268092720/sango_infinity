using UnityEngine;

namespace SangoTools.Editor
{
    /// <summary>
    /// 编辑器顶部菜单栏使用示例
    /// 展示如何集成到MapEditor中
    /// </summary>
    public class EditorTopMenuBarIntegration : MonoBehaviour
    {
        [Header("菜单栏引用")]
        [SerializeField] private EditorTopMenuBar menuBar;

        [Header("配置")]
        [SerializeField] private EditorTopMenuConfig menuConfig;

        private void Start()
        {
            SetupMenuBar();
        }

        private void SetupMenuBar()
        {
            if (menuBar == null)
            {
                menuBar = GetComponent<EditorTopMenuBar>();
            }

            if (menuBar == null)
            {
                Debug.LogError("EditorTopMenuBar not found!");
                return;
            }

            // 绑定菜单事件
            menuBar.OnMenuItemClicked += OnMenuItemClicked;
            menuBar.OnLayoutChanged += OnLayoutChanged;
            menuBar.OnSearchClicked += OnSearchClicked;
            menuBar.OnUserClicked += OnUserClicked;
            menuBar.OnSettingsClicked += OnSettingsClicked;
        }

        /// <summary>
        /// 处理菜单项点击
        /// </summary>
        private void OnMenuItemClicked(string actionId)
        {
            Debug.Log($"Menu item clicked: {actionId}");

            switch (actionId)
            {
                // 文件菜单
                case "File_NewMap":
                    HandleNewMap();
                    break;
                case "File_OpenMap":
                    HandleOpenMap();
                    break;
                case "File_SaveMap":
                    HandleSaveMap();
                    break;
                case "File_SaveAs":
                    HandleSaveAs();
                    break;
                case "File_LoadFromScenario":
                    HandleLoadFromScenario();
                    break;
                case "File_ExportScenario":
                    HandleExportScenario();
                    break;
                case "File_Settings":
                    HandleSettings();
                    break;
                case "File_Exit":
                    HandleExit();
                    break;

                // 编辑菜单
                case "Edit_Undo":
                    HandleUndo();
                    break;
                case "Edit_Redo":
                    HandleRedo();
                    break;
                case "Edit_History":
                    HandleToggleHistory();
                    break;
                case "Edit_ClearHistory":
                    HandleClearHistory();
                    break;
                case "Edit_LoadScenario":
                    HandleLoadScenario();
                    break;
                case "Edit_Reset":
                    HandleReset();
                    break;

                // 视图菜单
                case "View_FixedCamera":
                    HandleToggleFixedCamera();
                    break;
                case "View_ResetCamera":
                    HandleResetCamera();
                    break;
                case "View_FitToWindow":
                    HandleFitToWindow();
                    break;
                case "View_ShowGrid":
                    HandleToggleGrid();
                    break;
                case "View_ShowTerrain":
                    HandleToggleTerrain();
                    break;
                case "View_ShowBuildings":
                    HandleToggleBuildings();
                    break;
                case "View_ShowUnits":
                    HandleToggleUnits();
                    break;
                case "View_Season_Autumn":
                    HandleSetSeason(0);
                    break;
                case "View_Season_Spring":
                    HandleSetSeason(1);
                    break;
                case "View_Season_Summer":
                    HandleSetSeason(2);
                    break;
                case "View_Season_Winter":
                    HandleSetSeason(3);
                    break;

                // 渲染菜单
                case "Render_Lighting":
                    HandleLightingSettings();
                    break;
                case "Render_Fog":
                    HandleFogSettings();
                    break;
                case "Render_FogEnabled":
                    HandleToggleFog();
                    break;
                case "Render_Shadows":
                    HandleToggleShadows();
                    break;
                case "Render_AntiAliasing":
                    HandleToggleAA();
                    break;

                // 帮助菜单
                case "Help_Shortcuts":
                    HandleShowShortcuts();
                    break;
                case "Help_Tutorial":
                    HandleShowTutorial();
                    break;
                case "Help_CheckUpdate":
                    HandleCheckUpdate();
                    break;
                case "Help_About":
                    HandleShowAbout();
                    break;
            }
        }

        #region 文件菜单处理

        private void HandleNewMap()
        {
            // 显示新建地图窗口
            // windows.ShowNewMapWindow();
            Debug.Log("Show New Map Window");
        }

        private void HandleOpenMap()
        {
            // 打开地图
            // var path = WindowDialog.OpenFileDialog("地图文件(*.bin)\0*.bin;\0\0");
            Debug.Log("Open Map");
        }

        private void HandleSaveMap()
        {
            // 保存地图
            // var path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
            Debug.Log("Save Map");
        }

        private void HandleSaveAs()
        {
            // 另存为
            Debug.Log("Save As");
        }

        private void HandleLoadFromScenario()
        {
            Debug.Log("Load From Scenario");
        }

        private void HandleExportScenario()
        {
            Debug.Log("Export Scenario");
        }

        private void HandleSettings()
        {
            // 显示设置窗口
            Debug.Log("Show Settings");
        }

        private void HandleExit()
        {
            // 退出编辑器
            Debug.Log("Exit Editor");
            // Application.Quit();
        }

        #endregion

        #region 编辑菜单处理

        private void HandleUndo()
        {
            // editor.undoRedoManager.Undo();
            Debug.Log("Undo");
        }

        private void HandleRedo()
        {
            // editor.undoRedoManager.Redo();
            Debug.Log("Redo");
        }

        private void HandleToggleHistory()
        {
            // editor.operationHistoryWindow.ToggleWindow();
            Debug.Log("Toggle History");
        }

        private void HandleClearHistory()
        {
            // editor.undoRedoManager.ClearHistory();
            Debug.Log("Clear History");
        }

        private void HandleLoadScenario()
        {
            Debug.Log("Load Scenario");
        }

        private void HandleReset()
        {
            Debug.Log("Reset Editor");
        }

        #endregion

        #region 视图菜单处理

        private void HandleToggleFixedCamera()
        {
            // editor.ViewIs311Camera = !editor.ViewIs311Camera;
            Debug.Log("Toggle Fixed Camera");
        }

        private void HandleResetCamera()
        {
            // 重置相机位置
            Debug.Log("Reset Camera");
        }

        private void HandleFitToWindow()
        {
            // 适应窗口
            Debug.Log("Fit To Window");
        }

        private void HandleToggleGrid()
        {
            // 切换网格显示
            Debug.Log("Toggle Grid");
        }

        private void HandleToggleTerrain()
        {
            // 切换地形显示
            Debug.Log("Toggle Terrain");
        }

        private void HandleToggleBuildings()
        {
            // 切换建筑显示
            Debug.Log("Toggle Buildings");
        }

        private void HandleToggleUnits()
        {
            // 切换单位显示
            Debug.Log("Toggle Units");
        }

        private void HandleSetSeason(int season)
        {
            // editor.map.curSeason = season;
            // foreach (var brush in editor.brushes) brush.OnSeasonChanged(season);
            Debug.Log($"Set Season: {season}");
        }

        #endregion

        #region 渲染菜单处理

        private void HandleLightingSettings()
        {
            // windows.ShowLightWindow();
            Debug.Log("Show Lighting Settings");
        }

        private void HandleFogSettings()
        {
            // windows.ShowFogWindow();
            Debug.Log("Show Fog Settings");
        }

        private void HandleToggleFog()
        {
            // RenderSettings.fog = !RenderSettings.fog;
            Debug.Log("Toggle Fog");
        }

        private void HandleToggleShadows()
        {
            Debug.Log("Toggle Shadows");
        }

        private void HandleToggleAA()
        {
            Debug.Log("Toggle Anti-Aliasing");
        }

        #endregion

        #region 帮助菜单处理

        private void HandleShowShortcuts()
        {
            Debug.Log("Show Shortcuts");
        }

        private void HandleShowTutorial()
        {
            Debug.Log("Show Tutorial");
        }

        private void HandleCheckUpdate()
        {
            Debug.Log("Check Update");
        }

        private void HandleShowAbout()
        {
            Debug.Log("Show About");
        }

        #endregion

        #region 其他事件处理

        private void OnLayoutChanged(int layoutIndex)
        {
            Debug.Log($"Layout changed to: {layoutIndex}");
            // 根据布局索引调整编辑器UI布局
        }

        private void OnSearchClicked()
        {
            Debug.Log("Search clicked");
            // 显示搜索面板
        }

        private void OnUserClicked()
        {
            Debug.Log("User clicked");
            // 显示用户信息或登录面板
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            // 显示设置面板
        }

        #endregion
    }
}
