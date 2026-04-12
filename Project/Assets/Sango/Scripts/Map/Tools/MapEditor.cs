using RTEditor;
using Sango.Render;
using Sango.Tools.UndoRedo;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sango.Tools
{
    /// <summary>
    /// 地图编辑器
    /// </summary>
    public class MapEditor : Behaviour
    {
        int frameLimit = 60;
        float timeInterval;
        float currentTime;
        /// <summary>
        /// 编辑器是否启用
        /// </summary>
        public static bool IsEditOn { get; set; }
        public string WorkContent { set; get; }
        public string DefaultContentName { get { return "Default"; } }

        /// <summary>
        /// 自动保存相关变量
        /// </summary>
        private float autoSaveInterval = 5 * 60f; // 自动保存间隔（秒）
        private float autoSaveTimer = 0f; // 自动保存计时器
        private bool autoSaveEnabled = true; // 是否启用自动保存
        private string lastSavedPath = ""; // 上次保存的路径
        private int autoSaveLimit = 20; // 自动保存文件数量限制
        private List<string> autoSavePaths = new List<string>(); // 自动保存文件路径列表

        /// <summary>
        /// UI反馈相关变量
        /// </summary>
        private float saveNotificationTimer = 0f; // 保存通知显示时间
        private string saveNotificationMessage = ""; // 保存通知消息
        private bool showSaveNotification = false; // 是否显示保存通知

        public static Sango.Hexagon.Coord SelectedCoord { get; set; }

        /// <summary>
        /// 撤销/重做管理器
        /// </summary>
        public UndoRedoManager undoRedoManager { get; private set; }

        /// <summary>
        /// 编辑模式
        /// </summary>
        enum EditorModType : int
        {
            // 基础
            Base = 0,
            // 地形编辑
            Terrain,
            // 地格编辑
            Grid,
            // 模型
            Model,
            // 设置
            Setting
        }

        public Render.MapRender map;
        public MapData.VertexData[][] vertexMapData { get { return map.mapData.vertexDatas; } }
        public MapData mapData { get { return map.mapData; } }

        // 可编辑的物件层
        internal int rayCastLayer;
        internal int rayCastObjectLayer;

        // 编辑器UI框体范围
        internal UnityEngine.Rect windowRect = new UnityEngine.Rect(500, 400, 240, 100);

        BrushBase[] brushes;
        public TerrainBrush terrain_brush;
        internal GridBrush grid_brush;
        internal ModelBrush model_brush;

        public EditorWindow editorToolsBarWindow;
        public EditorWindow editorContentWindow;
        public OperationHistoryWindow operationHistoryWindow;
        private LayoutManager layoutManager;

        public virtual bool IsPointerOverUI()
        {
            return EditorWindow.IsPointOverUI() || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
        }

        private void Awake()
        {
            timeInterval = 1.0f / frameLimit;
            Path.Init();
            //Path.AddSearchPath("D:/project_tk/Build/Mods/CoreMap");
            string assetsPath = $"{Application.dataPath}/Mods/Content/Assets/Map/Default";
            Path.AddSearchPath(assetsPath, false);

            IsEditOn = true;

            // 初始化撤销/重做管理器
            undoRedoManager = new UndoRedoManager();

            // 创建笔刷
            terrain_brush = new TerrainBrush(this);
            grid_brush = new GridBrush(this);
            model_brush = new ModelBrush(this);
            brushes = new BrushBase[] { terrain_brush, grid_brush, model_brush };

            // 关闭游戏主相机
            Camera.main.gameObject.SetActive(false);

            // 加载模型编辑工具, 设置监听
            GameObject.Instantiate(Resources.Load("New(Singleton)RTEditor.RuntimeEditorApplication"));

            // 监听Gizmo变换消息，用于Undo/Redo
            EditorUndoRedoSystem.Instance.overrideAction = RegisterUndoRedoAction;
            EditorObjectSelection.Instance.IsOverUIHandler += IsPointerOverUI;

            // 创建工具栏窗口并限制在屏幕范围内
            windowRect = ConstrainWindowToScreen(windowRect);
            editorToolsBarWindow = EditorWindow.AddWindow(0, windowRect, DrawToolbarWindow, "地图编辑器");
            editorToolsBarWindow.canClose = false;

            // 创建属性窗口并限制在屏幕范围内
            windowRect = ConstrainWindowToScreen(windowRect);
            editorContentWindow = EditorWindow.AddWindow(1, windowRect, DrawContentWindow, "属性窗口");
            editorContentWindow.canClose = false;

            // 初始化操作历史窗口
            operationHistoryWindow = new OperationHistoryWindow(this);

            // 初始化布局管理器并加载布局
            layoutManager = new LayoutManager(this);
            layoutManager.LoadLayout();

            // 初始化自动保存路径列表
            InitializeAutoSavePaths();

            Sango.Core.GameController.Instance.DragMoveViewEnabled = false;

        }

        public Render.MapRender CreateEmptyMap(int w, int h)
        {
            map = MapRender.Instance;
            map.NewMap(w, h);

            return map;
        }

        protected void Start()
        {
            GameObject l = GameObject.Find("Directional Light");
            if (l != null) l.SetActive(false);
            l = GameObject.Find("post");
            if (l != null) l.SetActive(false);

            rayCastLayer = LayerMask.GetMask(new string[] { "Map" });
            rayCastObjectLayer = LayerMask.GetMask(new string[] { "Building" });

            // 创建空地图
            if (map == null)
                CreateEmptyMap(1024, 1024);

            // 设置基础可视化距离
            map.showLimitLength = 3500;
            map.mapFog.fogEnabled = false;

            Shader.EnableKeyword("SANGO_EDITOR");
            Shader.SetGlobalFloat("_BrushType", 0);
            Shader.SetGlobalFloat("_TerrainTypeShowFlag", 0);

            // 不使用游戏相机逻辑,采用另外的编辑机相机逻辑
            map.mapCamera.enabled = false;
            map.mapCamera.SetCamera(Camera.main);
            EditorFreeCamera editorfree = Camera.main.gameObject.AddComponent<Sango.Tools.EditorFreeCamera>();
            if (editorfree != null)
                editorfree.lookAt = map.mapCamera.GetCenterTransform();
            Camera.main.farClipPlane = 30000;
            SetModelSelectionMod(false);
            SetGizmoCameraEnable(false);
            EditorCameraExtend.Instance.Camera.farClipPlane = 30000;

            //EditorUndoRedoSystem.Instance.overrideAction = 

            Invoke("DelaySetFreeCamera", 0.1f);
        }

        void DelaySetFreeCamera()
        {
            map.mapCamera.position = new Vector3(0, 500, 0);
            map.mapCamera.lookRotate = new Vector3(90, -90, 0);
            viewIs311Camera = true;
            SetCameraControlType(1);
            Camera.main.gameObject.transform.position = map.mapCamera.position;
            Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, -90, 0);

            terrain_brush.AutoImportLayerTexture();

        }

        protected void OnDestroy()
        {
            IsEditOn = false;
            Shader.DisableKeyword("SANGO_EDITOR");
            EditorUndoRedoSystem.Instance.overrideAction = null ;
            EditorObjectSelection.Instance.IsOverUIHandler -= IsPointerOverUI;

            // 编辑器关闭时自动保存
            //if (!string.IsNullOrEmpty(lastSavedPath))
            //{
            //    try
            //    {
            //        map.SaveMap(lastSavedPath);
            //        SaveBaseMap(lastSavedPath);
            //        Sango.Log.Info($"编辑器关闭时自动保存到: {lastSavedPath}");
            //    }
            //    catch (System.Exception e)
            //    {
            //        Sango.Log.Error($"编辑器关闭时自动保存失败: {e.Message}");
            //    }
            //}
        }

        public void SetModelSelectionMod(bool b)
        {
            SceneGizmoExtend.Instance.enabled = false;
            if (SceneGizmoExtend.Instance._gizmoCamera != null)
                SceneGizmoExtend.Instance._gizmoCamera.enabled = false;
            RuntimeEditorApplication.Instance.enabled = b;
            EditorGizmoSystem.Instance.enabled = b;
            EditorObjectSelection.Instance.enabled = b;
            EditorUndoRedoSystem.Instance.enabled = b;
            EditorMeshDatabase.Instance.enabled = b;
            MessageListenerDatabase.Instance.enabled = b;
            InputDevice.Instance.enabled = b;
            //SceneGizmo.Instance.enabled = b;
        }
        void SetGizmoCameraEnable(bool b)
        {
            EditorCameraExtend.Instance.enabled = b;
        }

        public void RegisterUndoRedoAction(IUndoableAndRedoableAction action)
        {
            string desc = action.GetType().Name;
            if (action is PostObjectSelectionChangedAction)
                desc = "选取模型";
            else if (action is PostGizmoTransformedObjectsAction)
                desc = "移动模型";
            else if (action is ObjectDuplicationAction)
                desc = "复制模型";
            else if (action is DeleteSelectedObjectsAction)
                desc = "删除模型";
            else
                return;
            ModelEditCommand command = new ModelEditCommand(this, action, desc);
            undoRedoManager.AddCommand(command);
        }

        /// <summary>
        /// 设置相机控制模式: 0:自由相机模式 1,游戏相机模式 
        /// </summary>
        /// <param name="t"></param>
        public void SetCameraControlType(int t)
        {
            if (t == 0)
            {
                SetGizmoCameraEnable(true);
                EditorFreeCamera editorfree = Camera.main.gameObject.GetComponent<Sango.Tools.EditorFreeCamera>();
                if (editorfree != null)
                {
                    editorfree.enabled = false;
                }
                map.mapSkyBox.SetVisible(false);
                map.mapCamera.enabled = false;
            }
            else if (t == 1)
            {
                SetGizmoCameraEnable(false);

                EditorFreeCamera editorfree = Camera.main.gameObject.GetComponent<Sango.Tools.EditorFreeCamera>();
                if (editorfree != null)
                {
                    editorfree.enabled = false;
                    //editorfree.UpdateCamera();
                }
                map.mapSkyBox.SetVisible(true);
                map.mapCamera.enabled = true;

            }
        }

        public static float QueryHeight(Vector3 pos)
        {
            Vector3 begin = pos;
            begin.y = 500;
            Ray ray = new Ray(begin, Vector3.down);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 1000, LayerMask.GetMask("Map"), QueryTriggerInteraction.Ignore))
                return raycastHit.point.y;
            else return 0;
        }
        public static bool QueryHeight(Vector3 pos, out float height)
        {
            Vector3 begin = pos;
            begin.y = 500;
            Ray ray = new Ray(begin, Vector3.down);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 1000, LayerMask.GetMask("Map"), QueryTriggerInteraction.Ignore))
            {
                height = raycastHit.point.y;
                return true;
            }
            else
            {
                height = 0;
                return false;
            }
        }

        BrushBase CheckBrush()
        {
            BrushBase brush = null;
            switch ((EditorModType)currentEditMode)
            {
                case EditorModType.Base:
                    break;
                case EditorModType.Terrain:
                    {
                        brush = brushes[0];
                    }
                    break;
                case EditorModType.Grid:
                    {
                        brush = brushes[1];
                    }
                    break;
                case EditorModType.Model:
                    {
                        brush = brushes[2];
                    }
                    break;
                default:
                    break;
            }
            return brush;
        }

        public void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime < timeInterval)
                return;

            while (currentTime >= timeInterval)
                currentTime = currentTime - timeInterval;

            // 处理撤销/重做快捷键
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                undoRedoManager.Undo();
            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
            {
                undoRedoManager.Redo();
            }
            // 处理快捷保存快捷键
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                QuickSave();
            }

            // 自动保存逻辑
            if (autoSaveEnabled)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    AutoSave();
                    autoSaveTimer = 0f;
                }
            }

            // 保存通知更新
            if (showSaveNotification)
            {
                saveNotificationTimer += Time.deltaTime;
                if (saveNotificationTimer >= 3f) // 显示3秒
                {
                    showSaveNotification = false;
                    saveNotificationMessage = "";
                    saveNotificationTimer = 0f;
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, map.showLimitLength + 2000, rayCastLayer))
            {
                Sango.Hexagon.Hex hex = map.mapGrid.hexWorld.PositionToHex(hit.point);
                SelectedCoord = Sango.Hexagon.Coord.OffsetFromCube(hex);
            }
            BrushBase brush = CheckBrush();
            if (brush == null) return;
            brush.Update();
        }

        /// <summary>
        /// 快捷保存
        /// 功能：使用Ctrl+S快捷键快速保存地图到上次保存的路径
        /// 如果没有上次保存的路径，会自动弹出保存对话框
        /// </summary>
        private void QuickSave()
        {
            if (!string.IsNullOrEmpty(lastSavedPath))
            {
                // 保存地图和底图
                map.SaveMap(lastSavedPath);
                SaveBaseMap(lastSavedPath);
                // 显示保存通知
                string message = $"地图已快捷保存到: {System.IO.Path.GetFileName(lastSavedPath)}";
                Sango.Log.Info(message);
                ShowSaveNotification(message);
            }
            else
            {
                // 如果没有上次保存的路径，使用保存对话框
                string path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    // 更新上次保存路径并保存
                    lastSavedPath = path;
                    map.SaveMap(path);
                    SaveBaseMap(path);
                    // 显示保存通知
                    string message = $"地图已保存到: {System.IO.Path.GetFileName(path)}";
                    Sango.Log.Info(message);
                    ShowSaveNotification(message);
                }
            }
        }

        /// <summary>
        /// 自动保存
        /// 功能：定期自动保存地图到带auto_save后缀的文件
        /// 自动管理保存文件数量，超出限制时删除最旧的文件
        /// </summary>
        private void AutoSave()
        {
            if (!string.IsNullOrEmpty(lastSavedPath))
            {
                // 生成带auto_save后缀的文件名，包含时间戳确保唯一性
                // 获取原始文件名和扩展名
                string fileName = System.IO.Path.GetFileNameWithoutExtension(lastSavedPath);
                string extension = System.IO.Path.GetExtension(lastSavedPath);
                string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
                
                // 创建AutoSaveMap目录路径（与Map平级）
                string mapDirectory = System.IO.Path.GetDirectoryName(lastSavedPath);
                string contentDirectory = System.IO.Path.GetDirectoryName(mapDirectory);
                string autoSaveDirectory = System.IO.Path.Combine(contentDirectory, "AutoSaveMap");
                
                // 确保AutoSaveMap目录存在
                if (!System.IO.Directory.Exists(autoSaveDirectory))
                {
                    System.IO.Directory.CreateDirectory(autoSaveDirectory);
                }
                
                // 构建自动保存文件路径
                string autoSavePath = System.IO.Path.Combine(autoSaveDirectory, $"{fileName}_auto_save_{timestamp}{extension}");

                // 保存地图和底图
                map.SaveMap(autoSavePath);
                SaveBaseMap(autoSavePath);
                // 显示保存通知
                string message = $"地图已自动保存到: {System.IO.Path.GetFileName(autoSavePath)}";
                Sango.Log.Info(message);
                ShowSaveNotification(message);

                // 添加到自动保存列表
                autoSavePaths.Add(autoSavePath);

                // 清理超出限制的旧文件
                while (autoSavePaths.Count > autoSaveLimit)
                {
                    string oldestPath = autoSavePaths[0];
                    autoSavePaths.RemoveAt(0);

                    // 删除旧地图文件
                    if (System.IO.File.Exists(oldestPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldestPath);
                            Sango.Log.Info($"已删除旧的自动保存文件: {oldestPath}");
                        }
                        catch (System.Exception e)
                        {
                            Sango.Log.Error($"删除旧自动保存文件失败: {e.Message}");
                        }
                    }

                    // 删除对应的底图文件
                    try
                    {
                        string oldDir = System.IO.Path.GetDirectoryName(oldestPath);
                        string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldestPath);
                        // 移除_auto_save_时间戳后缀，获取基础地图名
                        int autoSaveIndex = oldFileName.LastIndexOf("_auto_save_");
                        if (autoSaveIndex > 0)
                        {
                            string baseMapName = oldFileName.Substring(0, autoSaveIndex);
                            string baseTexPath = System.IO.Path.Combine(oldDir, "..", "Assets", "Map", baseMapName, "BaseTex");
                            if (System.IO.Directory.Exists(baseTexPath))
                            {
                                System.IO.Directory.Delete(baseTexPath, true);
                                Sango.Log.Info($"已删除旧的自动保存底图文件夹: {baseTexPath}");
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Sango.Log.Error($"删除旧自动保存底图失败: {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 初始化自动保存路径列表
        /// </summary>
        private void InitializeAutoSavePaths()
        {
            autoSavePaths.Clear();
            
            // 如果有上次保存的路径，查找对应的AutoSaveMap目录
            if (!string.IsNullOrEmpty(lastSavedPath))
            {
                string mapDirectory = System.IO.Path.GetDirectoryName(lastSavedPath);
                string contentDirectory = System.IO.Path.GetDirectoryName(mapDirectory);
                string autoSaveDirectory = System.IO.Path.Combine(contentDirectory, "AutoSaveMap");
                
                if (System.IO.Directory.Exists(autoSaveDirectory))
                {
                    // 获取AutoSaveMap目录下的所有自动保存文件
                    string[] autoSaveFiles = System.IO.Directory.GetFiles(autoSaveDirectory, "*_auto_save_*.bin");
                    
                    if (autoSaveFiles.Length > 0)
                    {
                        // 按文件修改时间排序（最新的在最后）
                        System.Array.Sort(autoSaveFiles, (a, b) => 
                            System.IO.File.GetLastWriteTime(a).CompareTo(System.IO.File.GetLastWriteTime(b)));
                        
                        // 添加到自动保存路径列表
                        autoSavePaths.AddRange(autoSaveFiles);
                        
                        // 清理超出限制的旧文件
                        while (autoSavePaths.Count > autoSaveLimit)
                        {
                            string oldestPath = autoSavePaths[0];
                            autoSavePaths.RemoveAt(0);
                            
                            // 删除旧地图文件
                            if (System.IO.File.Exists(oldestPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldestPath);
                                    Sango.Log.Info($"已删除旧的自动保存文件: {oldestPath}");
                                }
                                catch (System.Exception e)
                                {
                                    Sango.Log.Error($"删除旧自动保存文件失败: {e.Message}");
                                }
                            }
                            
                            // 删除对应的底图文件
                            try
                            {
                                string oldDir = System.IO.Path.GetDirectoryName(oldestPath);
                                string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldestPath);
                                // 移除_auto_save_时间戳后缀，获取基础地图名
                                int autoSaveIndex = oldFileName.LastIndexOf("_auto_save_");
                                if (autoSaveIndex > 0)
                                {
                                    string baseMapName = oldFileName.Substring(0, autoSaveIndex);
                                    string baseTexPath = System.IO.Path.Combine(oldDir, "..", "Assets", "Map", baseMapName, "BaseTex");
                                    if (System.IO.Directory.Exists(baseTexPath))
                                    {
                                        System.IO.Directory.Delete(baseTexPath, true);
                                        Sango.Log.Info($"已删除旧的自动保存底图文件夹: {baseTexPath}");
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Sango.Log.Error($"删除旧自动保存底图失败: {e.Message}");
                            }
                        }
                        
                        Sango.Log.Info($"已加载 {autoSavePaths.Count} 个自动保存文件");
                    }
                }
            }
        }

        /// <summary>
        /// 保存底图
        /// </summary>
        /// <param name="mapPath">地图文件路径</param>
        private void SaveBaseMap(string mapPath)
        {
            int maxRetries = 3;
            int retryCount = 0;
            bool saveSuccess = false;

            while (!saveSuccess && retryCount < maxRetries)
            {
                try
                {
                    // 计算底图保存路径
                    string mapDirectory = System.IO.Path.GetDirectoryName(mapPath);
                    string mapName = System.IO.Path.GetFileNameWithoutExtension(mapPath);
                    // 移除_auto_save后缀（如果有）
                    if (mapName.Contains("_auto_save"))
                    {
                        mapName = mapName.Substring(0, mapName.IndexOf("_auto_save"));
                    }
                    string baseMapPath = System.IO.Path.Combine(mapDirectory, "..", "Assets", "Map", mapName, "BaseTex");

                    // 创建目录
                    System.IO.Directory.CreateDirectory(baseMapPath);

                    // 保存每个季节的底图
                    for (int season = 0; season < 4; season++)
                    {
                        if (terrain_brush != null && terrain_brush.baseMap != null && terrain_brush.baseMap[season] != null)
                        {
                            string baseMapFileName = System.IO.Path.Combine(baseMapPath, $"BaseMap{season}.png");
                            terrain_brush.SaveBaseTexture(baseMapFileName, season);
                        }
                    }

                    Sango.Log.Info($"底图已保存到: {baseMapPath}");
                    saveSuccess = true;
                }
                catch (System.Exception e)
                {
                    retryCount++;
                    Sango.Log.Error($"保存底图失败 (尝试 {retryCount}/{maxRetries}): {e.Message}");

                    if (retryCount >= maxRetries)
                    {
                        Sango.Log.Error("保存底图失败，已达到最大重试次数");
                    }
                    else
                    {
                        // 等待一小段时间后重试
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
        }

        /// <summary>
        /// 显示保存通知
        /// </summary>
        /// <param name="message">通知消息</param>
        private void ShowSaveNotification(string message)
        {
            saveNotificationMessage = message;
            showSaveNotification = true;
            saveNotificationTimer = 0f;
        }

        int currentEditMode = 0;
        private string[] toolbarTitle = new string[]
        {
            "基础编辑", "编辑地形", "编辑地格", "模型放置", "设置说明" //编辑地格 显示光环
        };
        private string[] toolbarSeason = new string[]
        {
            "秋", "春", "夏", "冬"
        };
        bool viewIs311Camera = true;
        void DrawToolbarWindow(int windowID, EditorWindow window)
        {
            // 绘制保存通知（放在最顶层，避免遮挡）
            if (showSaveNotification)
            {
                GUILayout.BeginArea(new UnityEngine.Rect(10, 10, 400, 60), GUI.skin.box);
                GUILayout.Label(saveNotificationMessage, GUILayout.ExpandWidth(true));
                GUILayout.EndArea();
            }

            // 当前鼠标格子信息
            GUILayout.Label($"当前鼠标格子:{{{SelectedCoord.col},{SelectedCoord.row}}}");
            GUILayout.Space(10);

            // 季节和视角控制组
            GUILayout.BeginHorizontal();
            GUILayout.Label("季节:", GUILayout.Width(40));
            int season = GUILayout.Toolbar(map.curSeason, toolbarSeason, GUILayout.ExpandWidth(false));
            if (season != map.curSeason)
            {
                map.curSeason = season;
                foreach (BrushBase brush in brushes)
                {
                    brush.OnSeasonChanged(season);
                }
            }

            GUILayout.Space(20);
            bool viewTpye = GUILayout.Toggle(viewIs311Camera, "固定视角");
            if (viewTpye != viewIs311Camera)
            {
                viewIs311Camera = viewTpye;
                if (viewIs311Camera)
                    SetCameraControlType(1);
                else
                    SetCameraControlType(0);
            }

            if (GUILayout.Button("重置相机", GUILayout.ExpandWidth(false)))
            {
                map.mapCamera.position = new Vector3(0, 500, 0);
                map.mapCamera.lookRotate = new Vector3(90, -90, 0);
                viewIs311Camera = false;
                SetCameraControlType(0);
                Camera.main.gameObject.transform.position = map.mapCamera.position;
                Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, -90, 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 地图操作组
            GUILayout.BeginHorizontal();
            GUILayout.Label("地图操作:", GUILayout.Width(60));

            if (GUILayout.Button("加载地图", GUILayout.ExpandWidth(true)))
            {
                string[] path = WindowDialog.OpenFileDialog("地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    string fName = path[0];
                    lastSavedPath = fName;
                    map.LoadMap(fName);
                    EditorFreeCamera editorfree = Camera.main.gameObject.GetComponent<Sango.Tools.EditorFreeCamera>();
                    if (editorfree != null)
                        editorfree.lookAt = map.mapCamera.GetCenterTransform();

                    BrushBase brush = CheckBrush();
                    if (brush == null) return;
                    brush.OnEnter();

                    if (viewIs311Camera)
                        SetCameraControlType(1);
                    else
                        SetCameraControlType(0);

                    Sango.Log.Info($"地图已加载: {fName}");
                }
            }

            if (GUILayout.Button("保存地图", GUILayout.ExpandWidth(true)))
            {
                string path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    lastSavedPath = path;
                    map.SaveMap(path);
                    SaveBaseMap(path);
                    string message = $"地图已保存到: {System.IO.Path.GetFileName(path)}";
                    Sango.Log.Info(message);
                    ShowSaveNotification(message);
                }
            }

            if (GUILayout.Button("放大2倍保存", GUILayout.ExpandWidth(true)))
            {
                string path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    map.SaveScaleMap(path, 2);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            // 编辑模式切换
            Color lastColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.7f, 1f); // 更美观的蓝色
            int editMode = GUILayout.Toolbar(currentEditMode, toolbarTitle, GUILayout.Height(35));
            if (editMode != currentEditMode)
            {
                editorContentWindow.windowRect.size = windowRect.size;
                BrushBase brush = CheckBrush();
                if (brush != null)
                    brush.Clear();
                currentEditMode = editMode;
                brush = CheckBrush();
                if (brush == null) return;
                brush.OnEnter();
                SetModelSelectionMod(currentEditMode == (int)EditorModType.Model);
                editorContentWindow.visible = true;
            }
            GUI.backgroundColor = lastColor;

            // 操作历史按钮
            GUILayout.Space(10);
            if (GUILayout.Button("操作历史", GUILayout.Height(30)))
            {
                operationHistoryWindow.ToggleWindow();
            }

            // 布局管理按钮
            GUILayout.Space(10);
            if (GUILayout.Button("保存布局", GUILayout.Height(30)))
            {
                layoutManager.SaveLayout();
                string message = "布局已保存";
                Sango.Log.Info(message);
                ShowSaveNotification(message);
            }

            // 快捷键提示
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("快捷键:", GUILayout.Width(60));
            GUILayout.Label("Ctrl+S 保存", GUILayout.Width(80));
            GUILayout.Label("Ctrl+Z 撤销", GUILayout.Width(80));
            GUILayout.Label("Ctrl+Y 重做", GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        void DrawContentWindow(int windowID, EditorWindow window)
        {
            switch (currentEditMode)
            {
                case 0:
                    OnGUI_Base();
                    break;
                case 1:
                    OnGUI_Edit_Terrain();
                    break;
                case 2:
                    OnGUI_Edit_Grid();
                    break;
                case 3:
                    OnGUI_Models();
                    break;
                case 4:
                    OnGUI_Setting();
                    break;
                default:
                    break;
            }
        }

        void OnGUI_Base()
        {
            GUI.changed = false;
            float v = EditorUtility.FloatField(map.showLimitLength, "可视距离");
            if (GUI.changed)
            {
                map.showLimitLength = v;
            }
            EditorUIDraw.OnGUI(map.mapCamera);

            EditorUIDraw.OnGUI(map.mapData);
            EditorUIDraw.OnGUI(map.mapGrid);
            EditorUIDraw.OnGUI(map.mapLight);
            EditorUIDraw.OnGUI(map.mapSkyBox);
            EditorUIDraw.OnGUI(map.mapFog);
        }

        void OnGUI_Edit_Terrain()
        {
            CheckBrush().OnGUI();
        }

        void OnGUI_Edit_Grid()
        {
            CheckBrush().OnGUI();
        }

        void OnGUI_Models()
        {
            CheckBrush().OnGUI();
        }

        void OnGUI_Setting()
        {
            // 操作说明部分
            GUILayout.BeginVertical("Box");
            GUILayout.Label("操作说明", GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
            GUILayout.Label("• 鼠标中键拖拽地图移动");
            GUILayout.Label("• 地形编辑模式: Ctrl按住连续绘制, Shift推平模式以鼠标点高度推平");
            GUILayout.Label("• 地格编辑模式: Alt取鼠标点格子值, Ctrl按住连续绘制");
            GUILayout.Label("• 模型编辑模式: 选中模型后 Q(无) W(平移) E(旋转) R(缩放)");
            GUILayout.Label("• 模型编辑模式: 鼠标右键或ESC取消选择, Delete删除选中模型");
            GUILayout.EndVertical();

            GUILayout.Space(15);

            // 快捷键部分
            GUILayout.BeginVertical("Box");
            GUILayout.Label("快捷键", GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("保存:", GUILayout.Width(60));
            GUILayout.Label("Ctrl+S");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("撤销:", GUILayout.Width(60));
            GUILayout.Label("Ctrl+Z");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("重做:", GUILayout.Width(60));
            GUILayout.Label("Ctrl+Y");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(15);

            // 自动保存设置部分
            GUILayout.BeginVertical("Box");
            GUILayout.Label("自动保存设置", GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            // 自动保存启用/禁用
            GUILayout.BeginHorizontal();
            GUILayout.Label("启用自动保存", GUILayout.Width(120));
            bool _autoSaveEnabled = GUILayout.Toggle(autoSaveEnabled, "");
            if (_autoSaveEnabled != autoSaveEnabled)
            {
                autoSaveEnabled = _autoSaveEnabled;
            }
            GUILayout.EndHorizontal();

            // 自动保存间隔
            GUILayout.BeginHorizontal();
            GUILayout.Label("自动保存间隔(分钟)", GUILayout.Width(140));
            float _autoSaveInterval = EditorUtility.FloatField(autoSaveInterval / 60f, GUILayout.MaxWidth(60));
            if (GUI.changed)
            {
                _autoSaveInterval = Mathf.Clamp(_autoSaveInterval, 1f, 60f);
                autoSaveInterval = _autoSaveInterval * 60f;
            }
            GUILayout.EndHorizontal();

            // 自动保存数量限制
            GUILayout.BeginHorizontal();
            GUILayout.Label("自动保存数量限制", GUILayout.Width(140));
            int _autoSaveLimit = EditorUtility.IntField(autoSaveLimit, GUILayout.MaxWidth(60));
            if (GUI.changed)
            {
                _autoSaveLimit = Mathf.Clamp(_autoSaveLimit, 1, 20);
                autoSaveLimit = _autoSaveLimit;
            }
            GUILayout.EndHorizontal();

            // 自动保存说明
            GUILayout.Space(10);
            GUILayout.Label("• 自动保存文件会添加_auto_save后缀");
            GUILayout.Label("• 默认最多保存20个自动保存文件");
            GUILayout.Label("• 超出数量限制时会自动删除最旧的文件");
            GUILayout.EndVertical();
        }


        /// <summary>
        /// 强制相机指向目标对象
        /// </summary>
        /// <param name="obj"></param>
        public void ForceCameraToGameObject(GameObject obj)
        {
            EditorObjectSelection.Instance.ClearSelection(false);
            EditorObjectSelection.Instance.AddObjectToSelection(obj, false);
            ForceCameraToPosition(obj.transform.position);
        }

        /// <summary>
        /// 强制相机指向目标点
        /// </summary>
        /// <param name="obj"></param>
        public void ForceCameraToPosition(Vector3 position)
        {
            map.mapCamera.position = position;
        }

        /// <summary>
        /// 将窗口限制在屏幕范围内
        /// </summary>
        /// <param name="windowRect">原始窗口位置</param>
        /// <returns>限制后的窗口位置</returns>
        private UnityEngine.Rect ConstrainWindowToScreen(UnityEngine.Rect windowRect)
        {
            // 获取屏幕尺寸
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 限制窗口位置，确保不超出屏幕边界
            float x = Mathf.Clamp(windowRect.x, 0, screenWidth - windowRect.width);
            float y = Mathf.Clamp(windowRect.y, 0, screenHeight - windowRect.height);

            return new UnityEngine.Rect(x, y, windowRect.width, windowRect.height);
        }
    }
}